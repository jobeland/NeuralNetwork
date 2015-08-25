using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedTraining;

namespace Trainer
{
    class Runner
    {
        static void Main(string[] args)
        {
            NeuralNetworkConfigurationSettings networkConfig = new NeuralNetworkConfigurationSettings
            {
                NumInputNeurons = 1,
                NumOutputNeurons = 1,
                NumHiddenLayers = 1,
                NumHiddenNeurons = 3,
                SummationFunction = new SimpleSummation(),
                ActivationFunction = new TanhActivationFunction()
            };
            GeneticAlgorithm evolver = new GeneticAlgorithm(1000, networkConfig);
            evolver.runEpoch();
        }
    }
}
