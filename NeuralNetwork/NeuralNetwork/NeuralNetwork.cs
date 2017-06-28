using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{

    [Serializable]
    public class NeuralNetwork : INeuralNetwork
    {
        public ILayer InputLayer { get; set; }
        public IList<ILayer> HiddenLayers { get; set; }
        public ILayer OutputLayer { get; set; }
        public IList<Synapse> Inputs { get; set; }
        public IList<Synapse> Outputs { get; set; }

        public NeuralNetwork(IList<Synapse> inputs, ILayer inputLayer, IList<ILayer> hiddenLayers, ILayer outputLayer, IList<Synapse> outputs)
        {
            Inputs = inputs;
            InputLayer = inputLayer;
            HiddenLayers = hiddenLayers;
            OutputLayer = outputLayer;
            Outputs = outputs;
        }

        public static INeuralNetwork GetInstance(IList<Synapse> inputs, ILayer inputLayer, IList<ILayer> hiddenLayers, ILayer outputLayer, IList<Synapse> outputs)
        {
            return new NeuralNetwork(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        public void SetInputs(double[] inputs)
        {
            if (inputs.Length != Inputs.Count)
            {
                throw new ArgumentException(string.Format("inputs of length: {0} does not match the number of input synapses: {1}", inputs.Length, Inputs.Count));
            }
            for (int i = 0; i < Inputs.Count; i++)
            {
                Inputs[i].Axon.ProcessSignal(inputs[i]);
            }
        }

        public virtual void Process()
        {
            InputLayer.Process();
            foreach (ILayer hiddenLayer in HiddenLayers)
            {
                hiddenLayer.Process();
            }
            OutputLayer.Process();
        }

        public double[] GetOutputs()
        {
            double[] outputs = new double[Outputs.Count];
            for(var i = 0; i < Outputs.Count; i++){
                outputs[i] = Outputs[i].Axon.Value;
            }
            return outputs;
        }

        public NeuralNetworkGene GetGenes()
        {
            return new NeuralNetworkGene
            {
                InputGene = InputLayer.GetGenes(),
                HiddenGenes = HiddenLayers.Select(l => l.GetGenes()).ToList(),
                OutputGene = OutputLayer.GetGenes()
            };
        }

        ///**
        // * First index is the list of all of the weight arrays for the hidden layer,
        // * second index is the List of all of the weight arrays for the output layer
        // * @return
        // */
        //public List<List<Double[]>> getWeightMatrix()
        //{
        //    List<Double[]> hiddenWeights = new List<Double[]>();
        //    foreach (Neuron n in HiddenLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            hiddenWeights.Add(neuron.Weights);
        //        }
        //    }
        //    List<Double[]> outputWeights = new List<Double[]>();
        //    foreach (Neuron n in OutputLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            outputWeights.Add(neuron.Weights);
        //        }
        //    }

        //    List<Double[]> biases = new List<Double[]>();
        //    int sizeHiddenBias = HiddenLayer.NeuronsInLayer.Count;
        //    Double[] hiddenBias = new Double[sizeHiddenBias];
        //    int i = 0;
        //    foreach (Neuron n in HiddenLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            hiddenBias[i++] = neuron.Bias;
        //        }
        //    }
        //    biases.Add(hiddenBias);


        //    int sizeoutBias = OutputLayer.NeuronsInLayer.Count;
        //    Double[] outBias = new Double[sizeoutBias];
        //    i = 0;
        //    foreach (Neuron n in OutputLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            outBias[i++] = neuron.Bias;
        //        }
        //    }
        //    biases.Add(outBias);

        //    List<List<Double[]>> weightsToReturn = new List<List<Double[]>>();
        //    weightsToReturn.Add(hiddenWeights);
        //    weightsToReturn.Add(outputWeights);
        //    weightsToReturn.Add(biases);
        //    return weightsToReturn;

        //}

        ///**
        // * First index is the list of all of the weight arrays for the hidden layer,
        // * second index is the List of all of the weight arrays for the output layer
        // * @return
        // */
        //public void setWeightMatrix(List<List<Double[]>> matrix)
        //{
        //    List<Double[]> hiddenWeights = matrix[0];
        //    List<Double[]> outputWeights = matrix[1];
        //    List<Double[]> biases = matrix[2];

        //    int index = 0;
        //    foreach (Neuron n in HiddenLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            neuron.Weights = hiddenWeights[index];
        //            neuron.Bias = biases[0][index];
        //            index++;
        //        }
        //    }
        //    index = 0;
        //    foreach (Neuron n in OutputLayer.NeuronsInLayer)
        //    {
        //        if (n.GetType() == typeof(ActiveNeuron))
        //        {
        //            ActiveNeuron neuron = (ActiveNeuron)n;
        //            neuron.Weights = outputWeights[index];
        //            neuron.Bias = biases[1][index];
        //            index++;
        //        }
        //    }
        //}

        //private int getIndexOfGreatestOutputNeuron()
        //{
        //    List<ActiveNeuron> neurons = OutputLayer.NeuronsInLayer;
        //    double maxOutput = Double.MinValue;
        //    int indexOfMax = 0;
        //    for (int i = 0; i < neurons.Count; i++)
        //    {
        //        if (neurons[i].Output > maxOutput)
        //        {
        //            maxOutput = neurons[i].Output;
        //            indexOfMax = i;
        //        }
        //    }
        //    return indexOfMax;
        //}
    }
}
