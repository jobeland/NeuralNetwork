using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{

    [Serializable]
    public class NeuralNetwork
    {

        private InputLayer InputLayer;
        private Layer HiddenLayer;
        private Layer OutputLayer;

        public NeuralNetwork(int numInput, int numHidden, int numOutput)
        {
            InputLayer = new InputLayer(numInput);
            List<Neuron> inputNeurons = InputLayer.NeuronsInLayer;
            HiddenLayer = new Layer(numHidden, inputNeurons);
            List<ActiveNeuron> hiddenNeurons = HiddenLayer.NeuronsInLayer;
            OutputLayer = new Layer(numOutput, hiddenNeurons, 0);
        }



        public void setInputs(double[] inputs)
        {
            InputLayer.setInputs(inputs);
        }

        public void calculate()
        {
            InputLayer.fireAll();
            HiddenLayer.fireAll();
            OutputLayer.fireAll();
        }

        public double getOutput()
        {
            //return getIndexOfGreatestOutputNeuron();
            return OutputLayer.NeuronsInLayer[0].Output;
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
                    hiddenWeights.add(neuron.Weights);
                }
            }
            List<Double[]> outputWeights = new List<Double[]>();
            foreach (Neuron n in OutputLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    outputWeights.add(neuron.Weights);
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
            biases.add(hiddenBias);


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
            biases.add(outBias);

            List<List<Double[]>> weightsToReturn = new List<List<Double[]>>();
            weightsToReturn.add(hiddenWeights);
            weightsToReturn.add(outputWeights);
            weightsToReturn.add(biases);
            return weightsToReturn;

        }

        /**
         * First index is the list of all of the weight arrays for the hidden layer,
         * second index is the List of all of the weight arrays for the output layer
         * @return
         */
        public void setWeightMatrix(List<List<Double[]>> matrix)
        {
            List<Double[]> hiddenWeights = matrix.get(0);
            List<Double[]> outputWeights = matrix.get(1);
            List<Double[]> biases = matrix.get(2);

            int index = 0;
            foreach (Neuron n in HiddenLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    neuron.Weights = hiddenWeights.get(index);
                    neuron.Bias = biases.get(0)[index];
                    index++;
                }
            }
            index = 0;
            foreach (Neuron n in OutputLayer.NeuronsInLayer)
            {
                if (n.GetType() == typeof(ActiveNeuron))
                {
                    ActiveNeuron neuron = (ActiveNeuron)n;
                    neuron.Weights = outputWeights.get(index);
                    neuron.Bias = biases.get(1)[index];
                    index++;
                }
            }
        }

        private int getIndexOfGreatestOutputNeuron()
        {
            List<ActiveNeuron> neurons = OutputLayer.NeuronsInLayer;
            double maxOutput = Double.NEGATIVE_INFINITY;
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
