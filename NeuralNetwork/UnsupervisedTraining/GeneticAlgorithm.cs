using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
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
        public NeuralNetwork[] NetsForGeneration { get; set; }
        public EvalWorkingSet History { get; set; }
        public IList<TrainingSession> _sessions { get; set; }
        private readonly IActivationFunction _activationFunction;
        public bool SavedAtleastOne = false;
        public static double MUTATE_CHANCE = 0.05;
        private object ObjectLock;


        private static int INPUT_NEURONS = 4;
        private static int HIDDEN_NEURONS = 3;
        private static int OUTPUT_NEURONS = 4;
        private static bool USE_MULTITHREADING = true;

        private static double HIGH_MUTATION = 0.5;
        private static double NORMAL_MUTATION = 0.05;

        public GeneticAlgorithm(int pop)
        {
            this.Population = pop;
            Evals = new double[pop];
            _activationFunction = new SechActivationFunction();
            NetsForGeneration = new NeuralNetwork[pop];
            _sessions = new List<TrainingSession>();
            for (int i = 0; i < pop; i++)
            {
                Evals[i] = -1;
                NetsForGeneration[i] = new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);//TODO: why is this a hardcoded value?
            }
            History = new EvalWorkingSet(50);//TODO: why is this a hardcoded value?
            ObjectLock = new object();
        }

        public void RunGeneration()
        {
            _sessions.Clear();
            for (int i = 0; i < NetsForGeneration.Length; i++)
            {
                _sessions.Add(new TrainingSession(NetsForGeneration[i], new Game(10, 10, 300), i));
                
            }
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
            for(int i = 0; i < _sessions.Count; i++)
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

                NeuralNetwork bestPerformer = getBestPerformer();
                //NNUtils.saveNetwork(bestPerformer,"TANHHidden4" + "Epoch" + epoch + "Eval" + ((int)getBestEvalOfGeneration()));
                // at end of epoch, save top 10% of neural networks
            }

        }

        private NeuralNetwork getBestPerformer()
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
            return NetsForGeneration[indicesToKeep[0]];
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

            List<NeuralNetwork> children = breed(indicesToKeep);
            List<NeuralNetwork> toKeep = keep(indicesToKeep);
            List<NeuralNetwork> mutated = mutate(indicesToKeep);
            List<NeuralNetwork> newSpecies = getNewNetworks();
            List<NeuralNetwork> allToAdd = new List<NeuralNetwork>();
            allToAdd.AddRange(newSpecies);
            allToAdd.AddRange(children);
            allToAdd.AddRange(mutated);
            allToAdd.AddRange(toKeep);


            for (int net = 0; net < allToAdd.Count; net++)
            {
                NetsForGeneration[net] = allToAdd[net];
            }

        }

        private List<NeuralNetwork> getNewNetworks()
        {
            int numToGen = (int)(Population * 0.1);
            List<NeuralNetwork> newNets = new List<NeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                NeuralNetwork newNet = new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);
                newNets.Add(newNet);
            }
            return newNets;
        }

        private List<NeuralNetwork> keep(int[] indicesToKeep)
        {
            List<NeuralNetwork> toKeep = new List<NeuralNetwork>();
            for (int i = 0; i < indicesToKeep.Length; i++)
            {
                NeuralNetwork goodPerformer = NetsForGeneration[indicesToKeep[i]];
                toKeep.Add(goodPerformer);
            }
            return toKeep;
        }

        private List<NeuralNetwork> mutate(int[] indicesToKeep)
        {
            int numToMutate = (int)(Population * 0.1);
            // chance of mutation is 5% for now
            int numMutated = 0;
            List<NeuralNetwork> mutated = new List<NeuralNetwork>();
            Random random = new Random();
            while (numMutated < numToMutate)
            {
                int i = new Random().Next(indicesToKeep.Length);
                NeuralNetwork goodPerformer = NetsForGeneration[indicesToKeep[i]];
                List<List<Double[]>> genes = goodPerformer.getWeightMatrix();
                List<List<Double[]>> childGenes = new List<List<Double[]>>();

                for (int layer = 0; layer < genes.Count; layer++)
                {
                    List<Double[]> motherLayer = genes[layer];
                    List<Double[]> childLayer = new List<Double[]>();
                    for (int n = 0; n < motherLayer.Count; n++)
                    {
                        Double[] motherNeuronWeights = motherLayer[n];
                        Double[] childNeuronWeights = new Double[motherNeuronWeights.Length];

                        for (int weightIndex = 0; weightIndex < childNeuronWeights.Length; weightIndex++)
                        {
                            if (new Random().NextDouble() > MUTATE_CHANCE)
                            {
                                childNeuronWeights[weightIndex] = motherNeuronWeights[weightIndex];
                            }
                            else
                            {
                                double val = new Random().NextDouble();
                                if (new Random().NextDouble() < 0.5)
                                {
                                    // 50% chance of being negative, being between -1 and 1
                                    val = 0 - val;
                                }
                                childNeuronWeights[weightIndex] = val;
                            }
                        }
                        childLayer.Add(childNeuronWeights);
                    }
                    childGenes.Add(childLayer);
                }

                NeuralNetwork child = new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);
                child.setWeightMatrix(childGenes);
                mutated.Add(child);
                numMutated++;
            }
            return mutated;
        }

        private List<NeuralNetwork> breed(int[] indicesToKeep)
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

            List<NeuralNetwork> children = new List<NeuralNetwork>();
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
                NeuralNetwork mother = NetsForGeneration[index1.Index];

                // choose father
                WeightedIndex index2 = chooseIndex(toChooseFrom);
                toChooseFrom.Remove(index2);
                NeuralNetwork father = NetsForGeneration[index2.Index];

                NeuralNetwork child = mate(mother, father);
                children.Add(child);
            }

            return children;

        }

        private NeuralNetwork mate(NeuralNetwork mother, NeuralNetwork father)
        {
            List<List<Double[]>> motherGenes = mother.getWeightMatrix();
            List<List<Double[]>> fatherGenes = father.getWeightMatrix();
            List<List<Double[]>> childGenes = new List<List<Double[]>>();
            for (int layer = 0; layer < motherGenes.Count; layer++)
            {
                List<Double[]> motherLayer = motherGenes[layer];
                List<Double[]> fatherLayer = fatherGenes[layer];
                List<Double[]> childLayer = new List<Double[]>();
                for (int n = 0; n < motherLayer.Count; n++)
                {
                    Double[] motherNeuronWeights = motherLayer[n];
                    Double[] fatherNeuronWeights = fatherLayer[n];
                    Double[] childNeuronWeights = new Double[motherNeuronWeights.Length];

                    for (int i = 0; i < childNeuronWeights.Length; i++)
                    {
                        if (new Random().NextDouble() > 0.5)
                        {
                            childNeuronWeights[i] = motherNeuronWeights[i];
                        }
                        else
                        {
                            childNeuronWeights[i] = fatherNeuronWeights[i];
                        }
                    }
                    childLayer.Add(childNeuronWeights);
                }
                childGenes.Add(childLayer);
            }
            NeuralNetwork child = new NeuralNetwork(INPUT_NEURONS, HIDDEN_NEURONS, OUTPUT_NEURONS, _activationFunction);
            child.setWeightMatrix(childGenes);
            return child;
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
