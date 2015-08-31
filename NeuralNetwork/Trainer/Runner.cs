using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
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
            GenerationConfigurationSettings generationSettings = new GenerationConfigurationSettings
            {
                UseMultithreading = true,
                GenerationPopulation = 100

            };
            EvolutionConfigurationSettings evolutionSettings = new EvolutionConfigurationSettings
            {
                NormalMutationRate = 0.05,
                HighMutationRate = 0.5,
                GenerationsPerEpoch = 100,
                NumEpochs = 1000
            };
            INeuralNetworkFactory factory = NeuralNetworkFactory.GetInstance(SomaFactory.GetInstance(networkConfig.SummationFunction), AxonFactory.GetInstance(networkConfig.ActivationFunction), SynapseFactory.GetInstance(new RandomWeightInitializer(new Random())), SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0)), new RandomWeightInitializer(new Random()));
            IBreeder breeder = new Breeder(factory);
            IMutator mutator = new Mutator(factory);
            IEvalWorkingSet history = new EvalWorkingSet(50);

            GeneticAlgorithm evolver = new GeneticAlgorithm(networkConfig, generationSettings, evolutionSettings, factory, breeder, mutator, history);
            evolver.runEpoch();
        }
    }
}
