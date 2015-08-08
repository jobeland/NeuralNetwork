using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{

    [Serializable]
    public class NeuralNetwork
    {

        private Layer InputLayer;
        private Layer HiddenLayer;
        private Layer OutputLayer;

        public NeuralNetwork(int numInput, int numHidden, int numOutput, IActivationFunction activationFunction)
        {
            InputLayer = new InputLayer(numInput);
            List<Neuron> inputNeurons = InputLayer.NeuronsInLayer;
            HiddenLayer = new Layer(numHidden, inputNeurons, activationFunction);
            List<ActiveNeuron> hiddenNeurons = HiddenLayer.NeuronsInLayer;
            OutputLayer = new Layer(numOutput, hiddenNeurons, 0, activationFunction);
        }



        public void setInputs(double[] inputs)
        {
            InputLayer.SetInputs(inputs);
        }

        public void CalculateActivation()
        {
            InputLayer.FireAll();
            HiddenLayer.FireAll();
            OutputLayer.FireAll();
        }

        public double[] GetOutput()
        {
            double[] outputs = new double[OutputLayer.NeuronsInLayer.Count];
            for(var i = 0; i < OutputLayer.NeuronsInLayer.Count; i++){
                outputs[i] = OutputLayer.NeuronsInLayer[i].Output;
            }
            return outputs;
        }

        /**
         * First index is the list of all of the weight arrays for the hidden layer,
         * second index is the List of all of the weight arrays for the output layer
         * @return
         */
        public List<List<Double[]>> getWeightMatrix()
        {
            List<Double[]> hiddenWeights = new List<Double[]>();
            foreach (Neuron n in HiddenLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    hiddenWeights.Add(neuron.Weights);
                }
            }
            List<Double[]> outputWeights = new List<Double[]>();
            foreach (Neuron n in OutputLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    outputWeights.Add(neuron.Weights);
                }
            }

            List<Double[]> biases = new List<Double[]>();
            int sizeHiddenBias = HiddenLayer.NeuronsInLayer.Count;
            Double[] hiddenBias = new Double[sizeHiddenBias];
            int i = 0;
            foreach (Neuron n in HiddenLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    hiddenBias[i++] = neuron.Bias;
                }
            }
            biases.Add(hiddenBias);


            int sizeoutBias = OutputLayer.NeuronsInLayer.Count;
            Double[] outBias = new Double[sizeoutBias];
            i = 0;
            foreach (Neuron n in OutputLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    outBias[i++] = neuron.Bias;
                }
            }
            biases.Add(outBias);

            List<List<Double[]>> weightsToReturn = new List<List<Double[]>>();
            weightsToReturn.Add(hiddenWeights);
            weightsToReturn.Add(outputWeights);
            weightsToReturn.Add(biases);
            return weightsToReturn;

        }

        /**
         * First index is the list of all of the weight arrays for the hidden layer,
         * second index is the List of all of the weight arrays for the output layer
         * @return
         */
        public void setWeightMatrix(List<List<Double[]>> matrix)
        {
            List<Double[]> hiddenWeights = matrix[0];
            List<Double[]> outputWeights = matrix[1];
            List<Double[]> biases = matrix[2];

            int index = 0;
            foreach (Neuron n in HiddenLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    neuron.Weights = hiddenWeights[index];
                    neuron.Bias = biases[0][index];
                    index++;
                }
            }
            index = 0;
            foreach (Neuron n in OutputLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    neuron.Weights = outputWeights[index];
                    neuron.Bias = biases[1][index];
                    index++;
                }
            }
        }

        private int getIndexOfGreatestOutputNeuron()
        {
            List<ActiveNeuron> neurons = OutputLayer.NeuronsInLayer;
            double maxOutput = Double.MinValue;
            int indexOfMax = 0;
            for (int i = 0; i < neurons.Count; i++)
            {
                if (neurons[i].Output > maxOutput)
                {
                    maxOutput = neurons[i].Output;
                    indexOfMax = i;
                }
            }
            return indexOfMax;
        }
    }
}
