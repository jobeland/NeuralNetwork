using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnsupervisedTraining.Evaluatable;
using UnsupervisedTraining.Evolution;
using UnsupervisedTraining.Utils;

namespace UnsupervisedTraining
{

    public class GeneticAlgorithm
    {
        private INeuralNetworkFactory _networkFactory;
        private readonly IEvalWorkingSet _history;
        private readonly IEvaluatableFactory _evaluatableFactory;

        private readonly NeuralNetworkConfigurationSettings _networkConfig;
        private readonly GenerationConfigurationSettings _generationConfig;
        private readonly EvolutionConfigurationSettings _evolutionConfig;

        private readonly IBreeder _breeder;
        private readonly IMutator _mutator;

        private double _mutateChance;
        private Generation _generation;


        public GeneticAlgorithm(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig, INeuralNetworkFactory networkFactory, IBreeder breeder, IMutator mutator, IEvalWorkingSet workingSet, IEvaluatableFactory evaluatableFactory)
        {
            _networkConfig = networkConfig;
            _generationConfig = generationConfig;
            _evolutionConfig = evolutionConfig;
            var sessions = new List<ITrainingSession>();
            _networkFactory = networkFactory;
            _breeder = breeder;
            _mutator = mutator;
            _history = workingSet;
            _evaluatableFactory = evaluatableFactory;
            for (int i = 0; i < _generationConfig.GenerationPopulation; i++)
            {
                var network = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons);
                sessions.Add(new TrainingSession(network, _evaluatableFactory.Create(network), i));
            }
            _generation = new Generation(sessions, _generationConfig);


        }

        public void runEpoch()
        {
            for (int epoch = 0; epoch < _evolutionConfig.NumEpochs; epoch++)
            {
                for (int generation = 0; generation < _evolutionConfig.GenerationsPerEpoch; generation++)
                {
                    if (epoch != 0 || generation != 0)
                    {
                        createNextGeneration();
                    }
                    _generation.Run();

                    var evals = _generation.GetEvalsForGeneration();

                    int count = 0;
                    for (int i = 0; i < evals.Length; i++)
                    {
                        count++;
                        LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("eval: {0}", evals[i]));
                    }
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("count: {0}", count));
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("Epoch: {0},  Generation: {1}", epoch, generation));

                }
                SaveBestPerformer(epoch);
            }
        }

        internal void SaveBestPerformer(int epoch)
        {
            ITrainingSession bestPerformer = _generation.GetBestPerformer();
            var saver = new NeuralNetworkSaver("\\networks");
            saver.SaveNeuralNetwork(bestPerformer.NeuralNet, bestPerformer.GetSessionEvaluation(), epoch);
        }

        private void createNextGeneration()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int numberOfTopPerformersToChoose = (int)(_generationConfig.GenerationPopulation * 0.50);
            int numToBreed = (int)(_generationConfig.GenerationPopulation * 0.35);
            int numToGen = (int)(_generationConfig.GenerationPopulation * 0.15);

            var sessions = _generation.GetBestPerformers(numberOfTopPerformersToChoose);

            _history.AddEval(sessions[0].GetSessionEvaluation());

            if (_history.IsStale())
            {
                _mutateChance = _evolutionConfig.HighMutationRate;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Eval history is stale, setting mutation to HIGH");
            }
            else
            {
                _mutateChance = _evolutionConfig.NormalMutationRate;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Mutation set to NORMAL");
            }

            IList<INeuralNetwork> children = _breeder.Breed(sessions, numToBreed);

            IList<INeuralNetwork> toKeep = sessions.Select(session => session.NeuralNet).ToList();
            int numToLiveOn = toKeep.Count / 10;
            IList<INeuralNetwork> liveOn = toKeep.Take(numToLiveOn).ToList();
            for (int i = 0; i < numToLiveOn; i++)
            {
                toKeep.RemoveAt(0);
            }
            IList<INeuralNetwork> newNetworks = getNewNetworks(numToGen);

            List<INeuralNetwork> toTryMutate = new List<INeuralNetwork>();
            toTryMutate.AddRange(toKeep);
            toTryMutate.AddRange(newNetworks);
            IList<INeuralNetwork> maybeMutated = _mutator.Mutate(toTryMutate, _mutateChance);

            List<INeuralNetwork> allToAdd = new List<INeuralNetwork>();
            allToAdd.AddRange(children);
            allToAdd.AddRange(maybeMutated);
            allToAdd.AddRange(liveOn);

            var newSessions = new List<ITrainingSession>();
            for (int net = 0; net < allToAdd.Count; net++)
            {
                newSessions.Add(new TrainingSession(allToAdd[net], _evaluatableFactory.Create(allToAdd[net]), net));
            }
            _generation = new Generation(newSessions, _generationConfig);

            watch.Stop();
            LoggerFactory.GetLogger().Log(LogLevel.Debug, string.Format("create generation runtime (sec): {0}", watch.Elapsed.TotalSeconds));
            watch.Reset();
        }

        private List<INeuralNetwork> getNewNetworks(int numToGen)
        {
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
