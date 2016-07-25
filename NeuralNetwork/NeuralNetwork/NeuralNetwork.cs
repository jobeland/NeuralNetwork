﻿using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{

    [Serializable]
    public class NeuralNetwork : INeuralNetwork
    {

        private ILayer _inputLayer;
        private IList<ILayer> _hiddenLayers;
        private ILayer _outputLayer;
        private IList<Synapse> _inputs;
        private IList<Synapse> _outputs;

        private NeuralNetwork(IList<Synapse> inputs, ILayer inputLayer, IList<ILayer> hiddenLayers, ILayer outputLayer, IList<Synapse> outputs)
        {
            _inputs = inputs;
            _inputLayer = inputLayer;
            _hiddenLayers = hiddenLayers;
            _outputLayer = outputLayer;
            _outputs = outputs;
        }

        public static INeuralNetwork GetInstance(IList<Synapse> inputs, ILayer inputLayer, IList<ILayer> hiddenLayers, ILayer outputLayer, IList<Synapse> outputs)
        {
            return new NeuralNetwork(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        public void SetInputs(double[] inputs)
        {
            if (inputs.Length != _inputs.Count)
            {
                throw new ArgumentException(string.Format("inputs of length: {0} does not match the number of input synapses: {1}", inputs.Length, _inputs.Count));
            }
            for (int i = 0; i < _inputs.Count; i++)
            {
                _inputs[i].Axon.ProcessSignal(inputs[i]);
            }
        }

        public void Process()
        {
            _inputLayer.Process();
            foreach (ILayer hiddenLayer in _hiddenLayers)
            {
                hiddenLayer.Process();
            }
            _outputLayer.Process();
        }

        public double[] GetOutputs()
        {
            double[] outputs = new double[_outputs.Count];
            for(var i = 0; i < _outputs.Count; i++){
                outputs[i] = _outputs[i].Axon.Value;
            }
            return outputs;
        }

        public NeuralNetworkGene GetGenes()
        {
            return new NeuralNetworkGene
            {
                InputGene = _inputLayer.GetGenes(),
                HiddenGenes = _hiddenLayers.Select(l => l.GetGenes()).ToList(),
                OutputGene = _outputLayer.GetGenes()
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
