using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using BasicGame;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{

    public class GeneticAlgorithm
    {
        private INeuralNetworkFactory _networkFactory;
        public EvalWorkingSet History { get; set; }

        private readonly NeuralNetworkConfigurationSettings _networkConfig;
        private readonly GenerationConfigurationSettings _generationConfig;
        private readonly EvolutionConfigurationSettings _evolutionConfig;

        private readonly Breeder _breeder;
        private readonly Mutator _mutator;

        private double _mutateChance;
        private Generation _generation;


        public GeneticAlgorithm(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, Breeder breeder, Mutator mutator)
        {
            _networkConfig = networkConfig;
            _generationConfig = generationConfig;
            _evolutionConfig = evolutionConfig;
            var sessions = new List<TrainingSession>();
            _networkFactory = networkFactory;
            for (int i = 0; i < _generationConfig.GenerationPopulation; i++)
            {
                sessions.Add(new TrainingSession(_networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons), new Game(10, 10, 300), i));
            }
            _generation = new Generation(sessions, _generationConfig);

            _breeder = breeder;
            _mutator = mutator;
            History = new EvalWorkingSet(50);//TODO: why is this a hardcoded value?
        }

        public void runEpoch()
        {
            for (int epoch = 0; epoch < _evolutionConfig.NumEpochs; epoch++)
            {
                for (int generation = 0; generation < _evolutionConfig.GenerationsPerEpoch; generation++)
                {

                    _generation.Run();

                    var evals = _generation.GetEvalsForGeneration();

                    int count = 0;
                    for (int i = 0; i < evals.Length; i++)
                    {
                        count++;
                        LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("eval: {0}", evals[i]));
                    }
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("count: {0}", count));

                    createNextGeneration();
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("Epoch: {0},  Generation: {1}", epoch, generation));

                }
                SaveBestPerformer(epoch);
            }
        }

        internal void SaveBestPerformer(int epoch)
        {
            TrainingSession bestPerformer = _generation.GetBestPerformer();
            var saver = new NeuralNetworkSaver("\\networks");
            saver.SaveNeuralNetwork(bestPerformer.NeuralNet, bestPerformer.GetSessionEvaluation(), epoch);
        }

        private void createNextGeneration()
        {
            /*
             * TODO: get top 10% of current generation, save them rank the top 10%
             * by giving them a weight (ie if top three had 25, 24, and 23 evals,
             * the weight for the 25 would be 25 / (25+24+23))
             * 
             * for a certain percentage of the new generation, create by breeding
             * choose 2 mates stochasticly, then mix their weights (stochastically
             * as well, 50/50 chance?) // 70%?
             * 
             *  for a certain percentage of the new
             * generation, keep top performers of old generation (again, chosen
             * stochastically) // 10%? so keep them all? 
             * 
             * for a certain percentage of
             * the new generation, mutate top performers of old generation (chosen
             * stochastically, mutate values chosen at random with 5% chance of mutation) // 20%?
             * 
             * Also add brand new ones just to mix things up a bit and prevent a local maxima?
             */

            int numberOfTopPerformersToChoose = (int)(_generationConfig.GenerationPopulation * 0.50);
            int numToBreed = (int)(_generationConfig.GenerationPopulation * 0.3);
            int numToMutate = (int)(_generationConfig.GenerationPopulation * 0.1);
            int numToGen = (int)(_generationConfig.GenerationPopulation * 0.1);

            var sessions = _generation.GetBestPerformers(numberOfTopPerformersToChoose);

            History.AddEval(sessions[0].GetSessionEvaluation());

            if (History.IsStale())
            {
                _mutateChance = _evolutionConfig.HighMutationRate;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Eval history is stale, setting mutation to HIGH");
            }
            else
            {
                _mutateChance = _evolutionConfig.NormalMutationRate;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Mutation set to NORMAL");
            }

            List<INeuralNetwork> children = _breeder.Breed(sessions, numToBreed);
            List<INeuralNetwork> toKeep = sessions.Select(session => session.NeuralNet).ToList();
            List<INeuralNetwork> mutated = _mutator.Mutate(sessions, numToMutate);
            List<INeuralNetwork> newNetworks = getNewNetworks(numToGen);
            List<INeuralNetwork> allToAdd = new List<INeuralNetwork>();
            allToAdd.AddRange(newNetworks);
            allToAdd.AddRange(children);
            allToAdd.AddRange(mutated);
            allToAdd.AddRange(toKeep);

            var newSessions = new List<TrainingSession>();
            for (int net = 0; net < allToAdd.Count; net++)
            {
                newSessions.Add(new TrainingSession(allToAdd[net], new Game(10, 10, 300), net));
            }
            _generation = new Generation(newSessions, _generationConfig);

        }

        private List<INeuralNetwork> getNewNetworks(int numToGen)
        {
            //TODO: have these new networks generated not according to config: random hidden neurons/layers, and random functions
            List<INeuralNetwork> newNets = new List<INeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                INeuralNetwork newNet = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons);
                newNets.Add(newNet);
            }
            return newNets;
        }       
    }
}
