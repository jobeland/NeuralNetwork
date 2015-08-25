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

        private double _mutateChance;
        private Generation _generation;
       

        public GeneticAlgorithm(NeuralNetworkConfigurationSettings networkConfig, GenerationConfigurationSettings generationConfig, EvolutionConfigurationSettings evolutionConfig)
        {
            _networkConfig = networkConfig;
            _generationConfig = generationConfig;
            _evolutionConfig = evolutionConfig;
            //NetsForGeneration = new NeuralNetwork[pop];
            var sessions = new List<TrainingSession>();
            _networkFactory = NeuralNetworkFactory.GetInstance(SomaFactory.GetInstance(_networkConfig.SummationFunction), AxonFactory.GetInstance(_networkConfig.ActivationFunction), SynapseFactory.GetInstance(new RandomWeightInitializer(new Random())), SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0)), new RandomWeightInitializer(new Random()));
            for (int i = 0; i < _generationConfig.GenerationPopulation; i++)
            {
                //NetsForGeneration[i] = _networkFactory.Create(INPUT_NEURONS, OUTPUT_NEURONS, NUM_HIDDEN_LAYERS, HIDDEN_NEURONS);// new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);//TODO: why is this a hardcoded value?
                sessions.Add(new TrainingSession(_networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons), new Game(10, 10, 300), i));
            }
            _generation = new Generation(sessions, _generationConfig);
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
            saver.SaveNeuralNetwork(bestPerformer._nn, bestPerformer.GetSessionEvaluation(), epoch);
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
            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
            {
                indicesToKeep[i] = i;
            }
            var evals = _generation.GetEvalsForGeneration();
            for (int performer = 0; performer < evals.Length; performer++)
            {
                double value = evals[performer];
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    if (value > evals[indicesToKeep[i]])
                    {
                        int newIndex = performer;
                        // need to shift all of the rest down now
                        for (int indexContinued = i; indexContinued < numberOfTopPerformersToChoose; indexContinued++)
                        {
                            int oldIndex = indicesToKeep[indexContinued];
                            indicesToKeep[indexContinued] = newIndex;
                            newIndex = oldIndex;
                        }
                        break;
                    }
                }
            }

            History.AddEval(evals[indicesToKeep[0]]);

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

            List<INeuralNetwork> children = breed(indicesToKeep);
            List<INeuralNetwork> toKeep = keep(indicesToKeep);
            List<INeuralNetwork> mutated = mutate(indicesToKeep);
            List<INeuralNetwork> newSpecies = getNewNetworks();
            List<INeuralNetwork> allToAdd = new List<INeuralNetwork>();
            allToAdd.AddRange(newSpecies);
            allToAdd.AddRange(children);
            allToAdd.AddRange(mutated);
            allToAdd.AddRange(toKeep);

            var sessions = new List<TrainingSession>();
            for (int net = 0; net < allToAdd.Count; net++)
            {
                sessions.Add(new TrainingSession(allToAdd[net], new Game(10, 10, 300), net));
            }
            _generation = new Generation(sessions, _generationConfig);

        }

        private List<INeuralNetwork> getNewNetworks()
        {
            int numToGen = (int)(_generationConfig.GenerationPopulation * 0.1);
            List<INeuralNetwork> newNets = new List<INeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                INeuralNetwork newNet = _networkFactory.Create(_networkConfig.NumInputNeurons, _networkConfig.NumOutputNeurons, _networkConfig.NumHiddenLayers, _networkConfig.NumHiddenNeurons);
                newNets.Add(newNet);
            }
            return newNets;
        }

        private List<INeuralNetwork> keep(int[] indicesToKeep)
        {
            List<INeuralNetwork> toKeep = new List<INeuralNetwork>();
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                INeuralNetwork goodPerformer = _generation._sessions[indicesToKeep[i]]._nn;
                toKeep.Add(goodPerformer);
            }
            return toKeep;
        }

        private List<INeuralNetwork> mutate(int[] indicesToKeep)
        {
            int numToMutate = (int)(_generationConfig.GenerationPopulation * 0.1);
            int numMutated = 0;
            List<INeuralNetwork> mutated = new List<INeuralNetwork>();
            Random random = new Random();
            while (numMutated < numToMutate)
            {
                int i = random.Next(indicesToKeep.Length);
                INeuralNetwork goodPerformer = _generation._sessions[indicesToKeep[i]]._nn;
                NeuralNetworkGene childGenes = goodPerformer.GetGenes();

                for (int n = 0; n < childGenes.InputGene.Neurons.Count; n++)
                {
                    var neuron = childGenes.InputGene.Neurons[n];
                    childGenes.InputGene.Neurons[n] = TryMutateNeuron(neuron, random);
                }

                for (int h = 0; h < childGenes.HiddenGenes.Count; h++)
                {
                    for (int j = 0; j < childGenes.HiddenGenes[h].Neurons.Count; j++)
                    {
                        var neuron = childGenes.HiddenGenes[h].Neurons[j];
                        childGenes.HiddenGenes[h].Neurons[j] = TryMutateNeuron(neuron, random);
                    }
                }
                mutated.Add(_networkFactory.Create(childGenes));
                numMutated++;
            }
            return mutated;
        }

        internal NeuronGene TryMutateNeuron(NeuronGene gene, Random random)
        {
            NeuronGene toReturn = new NeuronGene
            {
                Axon = new AxonGene
                {
                    Weights = new List<double>(),
                    ActivationFunction = gene.Axon.ActivationFunction
                },
                Soma = new SomaGene
                {
                    SummationFunction = gene.Soma.SummationFunction
                }
            };
            for (int j = 0; j < gene.Axon.Weights.Count; j++)
            {
                if (random.NextDouble() <= _mutateChance)
                {
                    double val = random.NextDouble();
                    if (random.NextDouble() < 0.5)
                    {
                        // 50% chance of being negative, being between -1 and 1
                        val = 0 - val;
                    }
                    toReturn.Axon.Weights.Add(val);
                }
                else
                {
                    toReturn.Axon.Weights.Add(gene.Axon.Weights[j]);
                }
            }
            if (random.NextDouble() <= _mutateChance)
            {
                double val = random.NextDouble();
                if (random.NextDouble() < 0.5)
                {
                    // 50% chance of being negative, being between -1 and 1
                    val = 0 - val;
                }
                toReturn.Soma.Bias = val;
            }
            else
            {
                toReturn.Soma.Bias = gene.Soma.Bias;
            }
            return gene;
        }

        private List<INeuralNetwork> breed(int[] indicesToKeep)
        {
            int numToBreed = (int)(_generationConfig.GenerationPopulation * 0.3);
            var evals = _generation.GetEvalsForGeneration();
            double sumOfAllEvals = 0;
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                sumOfAllEvals += evals[indicesToKeep[i]];
            }
            if (sumOfAllEvals <= 0)
            {
                sumOfAllEvals = 1;
            }

            List<INeuralNetwork> children = new List<INeuralNetwork>();
            for (int bred = 0; bred < numToBreed; bred++)
            {
                List<WeightedIndex> toChooseFrom = new List<WeightedIndex>();
                double cumulative = 0.0;
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    //TODO: this weight determination algorithm should be delegated
                    double value = evals[indicesToKeep[i]];
                    double weight = value / sumOfAllEvals;
                    WeightedIndex index = new WeightedIndex
                    {
                        Index = indicesToKeep[i],
                        Weight = weight,
                    };
                    toChooseFrom.Add(index);
                }

                toChooseFrom = toChooseFrom.OrderBy(index => index.Weight).ToList();
                foreach (WeightedIndex index in toChooseFrom)
                {
                    index.CumlativeWeight = cumulative;
                    cumulative += index.Weight;
                }

                // choose mother
                WeightedIndex index1 = chooseIndex(toChooseFrom);
                toChooseFrom.Remove(index1);
                INeuralNetwork mother = _generation._sessions[index1.Index]._nn;

                // choose father
                WeightedIndex index2 = chooseIndex(toChooseFrom);
                toChooseFrom.Remove(index2);
                INeuralNetwork father = _generation._sessions[index2.Index]._nn;

                INeuralNetwork child = mate(mother, father);
                children.Add(child);
            }

            return children;

        }

        private INeuralNetwork mate(INeuralNetwork mother, INeuralNetwork father)
        {
            NeuralNetworkGene motherGenes = mother.GetGenes();
            NeuralNetworkGene childFatherGenes = father.GetGenes();
            Random random = new Random();

            for (int n = 0; n < childFatherGenes.InputGene.Neurons.Count; n++)
            {
                var neuron = childFatherGenes.InputGene.Neurons[n];
                var motherNeuron = motherGenes.InputGene.Neurons[n];
                childFatherGenes.InputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
            }

            for (int h = 0; h < childFatherGenes.HiddenGenes.Count; h++)
            {
                for (int j = 0; j < childFatherGenes.HiddenGenes[h].Neurons.Count; j++)
                {
                    var neuron = childFatherGenes.HiddenGenes[h].Neurons[j];
                    var motherNeuron = motherGenes.HiddenGenes[h].Neurons[j];
                    childFatherGenes.HiddenGenes[h].Neurons[j] = BreedNeuron(neuron, motherNeuron, random);
                }
            }

            for (int n = 0; n < childFatherGenes.OutputGene.Neurons.Count; n++)
            {
                var neuron = childFatherGenes.OutputGene.Neurons[n];
                var motherNeuron = motherGenes.OutputGene.Neurons[n];
                childFatherGenes.OutputGene.Neurons[n] = BreedNeuron(neuron, motherNeuron, random);
            }

            INeuralNetwork child = _networkFactory.Create(childFatherGenes);
            return child;
        }

        internal NeuronGene BreedNeuron(NeuronGene father, NeuronGene mother, Random random)
        {
            NeuronGene toReturn = new NeuronGene
            {
                Axon = new AxonGene
                {
                    Weights = new List<double>()
                },
                Soma = new SomaGene()
            };
            for (int j = 0; j < father.Axon.Weights.Count; j++)
            {
                if (random.NextDouble() < 0.5)
                {
                    toReturn.Axon.Weights.Add(mother.Axon.Weights[j]);
                }
                else
                {
                    toReturn.Axon.Weights.Add(father.Axon.Weights[j]);
                }
            }
            if (random.NextDouble() < 0.5)
            {
                toReturn.Axon.ActivationFunction = mother.Axon.ActivationFunction;
            }
            else
            {
                toReturn.Axon.ActivationFunction = father.Axon.ActivationFunction;
            }
            if(random.NextDouble() < 0.5)
            {
                toReturn.Soma.SummationFunction = mother.Soma.SummationFunction;
            }
            else
            {
                toReturn.Soma.SummationFunction = father.Soma.SummationFunction;
            }
            if(random.NextDouble() < 0.5)
            {
                toReturn.Soma.Bias = mother.Soma.Bias;
            }
            else
            {
                toReturn.Soma.Bias = father.Soma.Bias;
            }
            return toReturn;
        }

        private WeightedIndex chooseIndex(List<WeightedIndex> indices)
        {
            double value = RandomGenerator.GetInstance().NextDouble() * indices[indices.Count - 1].CumlativeWeight;
            //Failsafe for odd case when value is very low. Needs a more permanent fix so as not to skew the selection towards lower, however slight           
            if (indices[0].CumlativeWeight > value)
            {
                return indices[0];
            }
            return indices.Last(index => index.CumlativeWeight <= value);
        }
    }

}
