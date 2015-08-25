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

        public static int GENERATIONS_PER_EPOCH = 100;
        public int Population { get; set; }
        public double[] Evals { get; set; }
        private INeuralNetworkFactory _networkFactory;
        public EvalWorkingSet History { get; set; }
        public IList<TrainingSession> _sessions { get; set; }
        private readonly IActivationFunction _activationFunction;
        private readonly ISummationFunction _summationFunction;
        public static double MUTATE_CHANCE = 0.05;


        private static int INPUT_NEURONS = 1;
        private static int HIDDEN_NEURONS = 3;
        private static int NUM_HIDDEN_LAYERS = 1;
        private static int OUTPUT_NEURONS = 1;
        private static bool USE_MULTITHREADING = true;

        private static double HIGH_MUTATION = 0.5;
        private static double NORMAL_MUTATION = 0.05;

        public GeneticAlgorithm(int pop)
        {
            this.Population = pop;
            Evals = new double[pop];
            _activationFunction = new TanhActivationFunction();
            _summationFunction = new SimpleSummation();
            //NetsForGeneration = new NeuralNetwork[pop];
            _sessions = new List<TrainingSession>();
            _networkFactory = NeuralNetworkFactory.GetInstance(SomaFactory.GetInstance(_summationFunction), AxonFactory.GetInstance(_activationFunction), SynapseFactory.GetInstance(new RandomWeightInitializer(new Random())), SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0)), new RandomWeightInitializer(new Random()));
            for (int i = 0; i < pop; i++)
            {
                Evals[i] = -1;
                //NetsForGeneration[i] = _networkFactory.Create(INPUT_NEURONS, OUTPUT_NEURONS, NUM_HIDDEN_LAYERS, HIDDEN_NEURONS);// new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);//TODO: why is this a hardcoded value?
                _sessions.Add(new TrainingSession(_networkFactory.Create(INPUT_NEURONS, OUTPUT_NEURONS, NUM_HIDDEN_LAYERS, HIDDEN_NEURONS), new Game(10, 10, 300), i));
            }
            History = new EvalWorkingSet(50);//TODO: why is this a hardcoded value?
        }

        public void RunGeneration()
        {
            //_sessions.Clear();
            //for (int i = 0; i < NetsForGeneration.Length; i++)
            //{
            //    _sessions.Add(new TrainingSession(NetsForGeneration[i], new Game(10, 10, 300), i));

            //}
            if (USE_MULTITHREADING)
            {
                Parallel.ForEach<TrainingSession>(_sessions, session =>
                    {
                        session.Run();
                    });
            }
            else
            {
                foreach (var session in _sessions)
                {
                    session.Run();
                }
            }
        }

        public void GetEvalsForGeneration()
        {
            //TODO: this shouldn't be in Evals anymore, but just called directly off of the training session
            for (int i = 0; i < _sessions.Count; i++)
            {
                Evals[i] = _sessions[i].GetSessionEvaluation();
            }
        }


        public void runEpoch()
        {
            for (int epoch = 0; epoch < 1000; epoch++)
            {
                for (int generation = 0; generation < GENERATIONS_PER_EPOCH; generation++)
                {
                    RunGeneration();

                    GetEvalsForGeneration();

                    int count = 0;
                    for (int i = 0; i < Evals.Length; i++)
                    {
                        count++;
                        LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("eval: {0}", Evals[i]));
                    }
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("count: {0}", count));



                    createNextGeneration();
                    LoggerFactory.GetLogger().Log(LogLevel.Info, string.Format("Epoch: {0},  Generation: {1}", epoch, generation));

                    //				 if(generation % 100 == 0){
                    //					 NeuralNetwork bestPerformer = getBestPerformer();
                    //						NNUtils.saveNetwork(bestPerformer);
                    //				 }

                }

                INeuralNetwork bestPerformer = getBestPerformer();

                var saver = new NeuralNetworkSaver("\\networks");
                saver.SaveNeuralNetwork(bestPerformer, getBestEvalOfGeneration(), epoch);
                //NNUtils.saveNetwork(bestPerformer,"TANHHidden4" + "Epoch" + epoch + "Eval" + ((int)getBestEvalOfGeneration()));
                // at end of epoch, save top 10% of neural networks
            }

        }

        private INeuralNetwork getBestPerformer()
        {
            int numberOfTopPerformersToChoose = (int)(Population * 0.50);
            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
            {
                indicesToKeep[i] = i;
            }
            for (int performer = 0; performer < Evals.Length; performer++)
            {
                double value = Evals[performer];
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    if (value > Evals[indicesToKeep[i]])
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
            return _sessions[indicesToKeep[0]]._nn;
        }

        private double getBestEvalOfGeneration()
        {
            int numberOfTopPerformersToChoose = (int)(Population * 0.50);
            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
            {
                indicesToKeep[i] = i;
            }
            for (int performer = 0; performer < Evals.Length; performer++)
            {
                double value = Evals[performer];
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    if (value > Evals[indicesToKeep[i]])
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
            return Evals[indicesToKeep[0]];
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

            int numberOfTopPerformersToChoose = (int)(Population * 0.50);
            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
            {
                indicesToKeep[i] = i;
            }
            for (int performer = 0; performer < Evals.Length; performer++)
            {
                double value = Evals[performer];
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    if (value > Evals[indicesToKeep[i]])
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

            //for(int i = indicesToKeep.Length -1; i >= 0 ; i--){
            //Console.WriteLine("eval: " + Evals[indicesToKeep[i]]);
            //}
            //		 Console.WriteLine("eval: " + evals[indicesToKeep[0]]);

            History.AddEval(Evals[indicesToKeep[0]]);
            //		 if(evals[indicesToKeep[0]] >= 100 && !savedAtleastOne){
            //			 NNUtils.saveNetwork(netsForGeneration[indicesToKeep[0]], "TANHHidden4" + "Eval" + evals[indicesToKeep[0]]);
            //			 savedAtleastOne = true;
            //		 }
            if (History.IsStale())
            {
                MUTATE_CHANCE = HIGH_MUTATION;
                LoggerFactory.GetLogger().Log(LogLevel.Info, "Eval history is stale, setting mutation to HIGH");
            }
            else
            {
                MUTATE_CHANCE = NORMAL_MUTATION;
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

            _sessions.Clear();
            for (int net = 0; net < allToAdd.Count; net++)
            {
                //NetsForGeneration[net] = allToAdd[net];
                _sessions.Add(new TrainingSession(allToAdd[net], new Game(10, 10, 300), net));
            }

        }

        private List<INeuralNetwork> getNewNetworks()
        {
            int numToGen = (int)(Population * 0.1);
            List<INeuralNetwork> newNets = new List<INeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                INeuralNetwork newNet = _networkFactory.Create(INPUT_NEURONS, OUTPUT_NEURONS, NUM_HIDDEN_LAYERS, HIDDEN_NEURONS);
                newNets.Add(newNet);
            }
            return newNets;
        }

        private List<INeuralNetwork> keep(int[] indicesToKeep)
        {
            List<INeuralNetwork> toKeep = new List<INeuralNetwork>();
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                INeuralNetwork goodPerformer = _sessions[indicesToKeep[i]]._nn;
                toKeep.Add(goodPerformer);
            }
            return toKeep;
        }

        private List<INeuralNetwork> mutate(int[] indicesToKeep)
        {
            int numToMutate = (int)(Population * 0.1);
            // chance of mutation is 5% for now
            int numMutated = 0;
            List<INeuralNetwork> mutated = new List<INeuralNetwork>();
            Random random = new Random();
            while (numMutated < numToMutate)
            {
                int i = new Random().Next(indicesToKeep.Length);
                INeuralNetwork goodPerformer = _sessions[indicesToKeep[i]]._nn;
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
                if (random.NextDouble() < MUTATE_CHANCE)
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
            if (random.NextDouble() < MUTATE_CHANCE)
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
            int numToBreed = (int)(Population * 0.3);
            double sumOfAllEvals = 0;
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                sumOfAllEvals += Evals[indicesToKeep[i]];
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
                    double value = Evals[indicesToKeep[i]];
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
                INeuralNetwork mother = _sessions[index1.Index]._nn;

                // choose father
                WeightedIndex index2 = chooseIndex(toChooseFrom);
                toChooseFrom.Remove(index2);
                INeuralNetwork father = _sessions[index2.Index]._nn;

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

        public static void main(String[] args)
        {
            GeneticAlgorithm evolver = new GeneticAlgorithm(500);
            evolver.runEpoch();
        }
    }

}
