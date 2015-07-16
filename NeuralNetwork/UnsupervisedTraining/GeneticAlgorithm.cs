using ArtificialNeuralNetwork;
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
        public int CompletedThisGeneration { get; set; }
        public double[] Evals { get; set; }
        public NeuralNetwork[] NetsForGeneration { get; set; }
        public EvalWorkingSet History { get; set; }
        public bool SavedAtleastOne = false;
        public static double MUTATE_CHANCE = 0.05;
        private object ObjectLock;

        public GeneticAlgorithm(int pop)
        {
            this.Population = pop;
            this.CompletedThisGeneration = 0;
            Evals = new double[pop];
            NetsForGeneration = new NeuralNetwork[pop];
            for (int i = 0; i < pop; i++)
            {
                Evals[i] = -1;
                NetsForGeneration[i] = new NeuralNetwork(5, 4, 1);//TODO: why is this a hardcoded value?
            }
            History = new EvalWorkingSet(50);//TODO: why is this a hardcoded value?
            ObjectLock = new object();
        }

        public void AddEval(int index, double value)
        {
            //TODO: this method should accept index I don't think, seems like this class should own that.
            lock (ObjectLock)
            {
                Evals[index] = value;
                CompletedThisGeneration++;
            }
        }

        public void runGeneration()
        {
            //TODO: replace threads with a thread pool
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < NetsForGeneration.Length; i++)
            {
                var trainingThread = new TrainingThread(NetsForGeneration[i], i, this);
                Thread newThread = new Thread(new ThreadStart(trainingThread.ThreadRun));
                newThread.Start();
                threads.Add(newThread);
            }

            // Spin for a while waiting for the started thread to become
            // alive:
            while (threads.Any(t => !t.IsAlive))
            {
                Thread.Sleep(1);
            };

            foreach (Thread t in threads)
            {
                t.Join();
            }

        }

        public void runEpoch() {
                for(int epoch = 0; epoch < 1000; epoch++){
                    for (int generation = 0; generation < GENERATIONS_PER_EPOCH; generation++) {
                        runGeneration();
                        while (!generationFinished()) {
                            try {
                                Thread.Sleep(100);
                            } catch (Exception e) {
                                // TODO Auto-generated catch block
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                         int count = 0;
        				 for(int i = 0; i < Evals.Length; i++){
        				 count++;
        				 Console.WriteLine("eval: " + Evals[i]);
        				 }
        				 Console.WriteLine("count: " + count);



                        createNextGeneration();
                         Console.WriteLine("Epoch: " + epoch + ",  Generation: " + generation);
                         Console.WriteLine("-----------------------------------");

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

        private void createNextGeneration() {
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

                int numberOfTopPerformersToChoose = (int) (Population * 0.50);
                int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
                for (int i = 0; i < numberOfTopPerformersToChoose; i++) {
                    indicesToKeep[i] = i;
                }
                for (int performer = 0; performer < Evals.Length; performer++) {
                    double value = Evals[performer];
                    for (int i = 0; i < indicesToKeep.Length; i++) {
                        if (value > Evals[indicesToKeep[i]]) {
                            int newIndex = performer;
                            // need to shift all of the rest down now
                            for (int indexContinued = i; indexContinued < numberOfTopPerformersToChoose; indexContinued++) {
                                int oldIndex = indicesToKeep[indexContinued];
                                indicesToKeep[indexContinued] = newIndex;
                                newIndex = oldIndex;
                            }
                            break;
                        }
                    }
                }

                 for(int i = indicesToKeep.Length -1; i >= 0 ; i--){
                 Console.WriteLine("eval: " + Evals[indicesToKeep[i]]);
                 }
        //		 Console.WriteLine("eval: " + evals[indicesToKeep[0]]);
                 Console.WriteLine("-------------------------------------------------");

                 History.AddEval(Evals[indicesToKeep[0]]);
        //		 if(evals[indicesToKeep[0]] >= 100 && !savedAtleastOne){
        //			 NNUtils.saveNetwork(netsForGeneration[indicesToKeep[0]], "TANHHidden4" + "Eval" + evals[indicesToKeep[0]]);
        //			 savedAtleastOne = true;
        //		 }
                 if(History.IsStale()){
                     Console.WriteLine("MUTATION ON HIGH");
                     MUTATE_CHANCE = 0.5;
                 }else{
                     MUTATE_CHANCE = 0.05;
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


                for(int net = 0; net < allToAdd.Count; net++){
                    NetsForGeneration[net] = allToAdd[net];
                }

            }

        private List<NeuralNetwork> getNewNetworks()
        {
            int numToGen = (int)(Population * 0.1);
            List<NeuralNetwork> newNets = new List<NeuralNetwork>();
            for (int i = 0; i < numToGen; i++)
            {
                NeuralNetwork newNet = new NeuralNetwork(5, 4, 1); // CAREFUL THIS IS HARD CODED!
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

                NeuralNetwork child = new NeuralNetwork(5, 4, 1); // CAREFUL THESE ARE HARDCODED!
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

            List<NeuralNetwork> children = new List<NeuralNetwork>();
            for (int bred = 0; bred < numToBreed; bred++)
            {
                List<WeightedIndex> toChooseFrom = new List<WeightedIndex>();
                for (int i = 0; i < indicesToKeep.Length; i++)
                {
                    //TODO: this weight determination algorithm should be delegated
                    double value = Evals[indicesToKeep[i]];
                    double weight = value / sumOfAllEvals;
                    WeightedIndex index = new WeightedIndex
                    {
                        Index = indicesToKeep[i],
                        Weight = weight
                    };
                    toChooseFrom.Add(index);
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
            NeuralNetwork child = new NeuralNetwork(5, 4, 1); // CAREFUL THESE ARE HARDCODED! TODO: remove hardcoded
            child.setWeightMatrix(childGenes);
            return child;
        }

        private WeightedIndex chooseIndex(List<WeightedIndex> indices) {
                while (true) {
                    foreach (WeightedIndex index in indices) {
                        double random = new Random().NextDouble();
                        if (random > (1 - index.Weight)) {
                            return index;
                        }
                    }
                }

            }

        public bool generationFinished()
        {
            var isDone = false;
            lock (ObjectLock)
            {
                if (CompletedThisGeneration == Population)
                {
                    CompletedThisGeneration = 0;
                    isDone = true;
                }
            }

            return isDone;
        }

        public static void main(String[] args)
        {
            GeneticAlgorithm evolver = new GeneticAlgorithm(500);
            evolver.runEpoch();

            // evolver.runGeneration();
            // while(!evolver.generationFinished()){
            // try {
            // Thread.sleep(500);
            // } catch (InterruptedException e) {
            // // TODO Auto-generated catch block
            // e.printStackTrace();
            // }
            // }
            // int count = 0;
            // int[] results = evolver.getEvals();
            // for(int i = 0; i < results.length; i++){
            // count++;
            // Console.WriteLine("eval: " + results[i]);
            // }
            // Console.WriteLine("count: " + count);
        }
    }

}
