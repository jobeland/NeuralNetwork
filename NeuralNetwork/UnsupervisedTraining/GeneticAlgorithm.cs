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
        }

        public void AddEval(int index, double value)
        {
            lock (Evals)
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

        //        public void runEpoch() {
        //        for(int epoch = 0; epoch < 1000; epoch++){
        //            for (int generation = 0; generation < GENERATIONS_PER_EPOCH; generation++) {
        //                runGeneration();
        //                while (!generationFinished()) {
        //                    try {
        //                        Thread.sleep(100);
        //                    } catch (InterruptedException e) {
        //                        // TODO Auto-generated catch block
        //                        e.printStackTrace();
        //                    }
        //                }
        //                 int count = 0;
        ////				 for(int i = 0; i < evals.length; i++){
        ////				 count++;
        ////				 System.out.println("eval: " + evals[i]);
        ////				 }
        ////				 System.out.println("count: " + count);



        //                createNextGeneration();
        //                 System.out.println("Epoch: " + epoch + ",  Generation: " + generation);
        //                 System.out.println("-----------------------------------");

        ////				 if(generation % 100 == 0){
        ////					 NeuralNetwork bestPerformer = getBestPerformer();
        ////						NNUtils.saveNetwork(bestPerformer);
        ////				 }

        //            }

        //            NeuralNetwork bestPerformer = getBestPerformer();
        //            NNUtils.saveNetwork(bestPerformer,"TANHHidden4" + "Epoch" + epoch + "Eval" + ((int)getBestEvalOfGeneration()));
        //            // at end of epoch, save top 10% of neural networks
        //        }

        //    }

        //        private NeuralNetwork getBestPerformer()
        //        {
        //            int numberOfTopPerformersToChoose = (int)(Population * 0.50);
        //            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
        //            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
        //            {
        //                indicesToKeep[i] = i;
        //            }
        //            for (int performer = 0; performer < Evals.length; performer++)
        //            {
        //                double value = Evals[performer];
        //                for (int i = 0; i < indicesToKeep.length; i++)
        //                {
        //                    if (value > Evals[indicesToKeep[i]])
        //                    {
        //                        int newIndex = performer;
        //                        // need to shift all of the rest down now
        //                        for (int indexContinued = i; indexContinued < numberOfTopPerformersToChoose; indexContinued++)
        //                        {
        //                            int oldIndex = indicesToKeep[indexContinued];
        //                            indicesToKeep[indexContinued] = newIndex;
        //                            newIndex = oldIndex;
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            return NetsForGeneration[indicesToKeep[0]];
        //        }

        //        private double getBestEvalOfGeneration()
        //        {
        //            int numberOfTopPerformersToChoose = (int)(Population * 0.50);
        //            int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
        //            for (int i = 0; i < numberOfTopPerformersToChoose; i++)
        //            {
        //                indicesToKeep[i] = i;
        //            }
        //            for (int performer = 0; performer < Evals.length; performer++)
        //            {
        //                double value = Evals[performer];
        //                for (int i = 0; i < indicesToKeep.length; i++)
        //                {
        //                    if (value > Evals[indicesToKeep[i]])
        //                    {
        //                        int newIndex = performer;
        //                        // need to shift all of the rest down now
        //                        for (int indexContinued = i; indexContinued < numberOfTopPerformersToChoose; indexContinued++)
        //                        {
        //                            int oldIndex = indicesToKeep[indexContinued];
        //                            indicesToKeep[indexContinued] = newIndex;
        //                            newIndex = oldIndex;
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            return Evals[indicesToKeep[0]];
        //        }

        //        private void createNextGeneration() {
        //        /*
        //         * TODO: get top 10% of current generation, save them rank the top 10%
        //         * by giving them a weight (ie if top three had 25, 24, and 23 evals,
        //         * the weight for the 25 would be 25 / (25+24+23))
        //         * 
        //         * for a certain percentage of the new generation, create by breeding
        //         * choose 2 mates stochasticly, then mix their weights (stochastically
        //         * as well, 50/50 chance?) // 70%?
        //         * 
        //         *  for a certain percentage of the new
        //         * generation, keep top performers of old generation (again, chosen
        //         * stochastically) // 10%? so keep them all? 
        //         * 
        //         * for a certain percentage of
        //         * the new generation, mutate top performers of old generation (chosen
        //         * stochastically, mutate values chosen at random with 5% chance of mutation) // 20%?
        //         */

        //        int numberOfTopPerformersToChoose = (int) (Population * 0.50);
        //        int[] indicesToKeep = new int[numberOfTopPerformersToChoose];
        //        for (int i = 0; i < numberOfTopPerformersToChoose; i++) {
        //            indicesToKeep[i] = i;
        //        }
        //        for (int performer = 0; performer < Evals.length; performer++) {
        //            double value = Evals[performer];
        //            for (int i = 0; i < indicesToKeep.length; i++) {
        //                if (value > Evals[indicesToKeep[i]]) {
        //                    int newIndex = performer;
        //                    // need to shift all of the rest down now
        //                    for (int indexContinued = i; indexContinued < numberOfTopPerformersToChoose; indexContinued++) {
        //                        int oldIndex = indicesToKeep[indexContinued];
        //                        indicesToKeep[indexContinued] = newIndex;
        //                        newIndex = oldIndex;
        //                    }
        //                    break;
        //                }
        //            }
        //        }

        //         for(int i = indicesToKeep.length -1; i >= 0 ; i--){
        //         System.out.println("eval: " + Evals[indicesToKeep[i]]);
        //         }
        ////		 System.out.println("eval: " + evals[indicesToKeep[0]]);
        //         System.out.println("-------------------------------------------------");

        //         History.AddEval(Evals[indicesToKeep[0]]);
        ////		 if(evals[indicesToKeep[0]] >= 100 && !savedAtleastOne){
        ////			 NNUtils.saveNetwork(netsForGeneration[indicesToKeep[0]], "TANHHidden4" + "Eval" + evals[indicesToKeep[0]]);
        ////			 savedAtleastOne = true;
        ////		 }
        //         if(History.IsStale()){
        //             System.out.println("MUTATION ON HIGH");
        //             MUTATE_CHANCE = 0.5;
        //         }else{
        //             MUTATE_CHANCE = 0.05;
        //         }

        //        ArrayList<NeuralNetwork> children = breed(indicesToKeep);
        //        ArrayList<NeuralNetwork> keep = keep(indicesToKeep);
        //        ArrayList<NeuralNetwork> mutated = mutate(indicesToKeep);
        //        ArrayList<NeuralNetwork> newSpecies = getNewNetworks();
        //        ArrayList<NeuralNetwork> allToAdd = new ArrayList<NeuralNetwork>();
        //        allToAdd.addAll(newSpecies);
        //        allToAdd.addAll(children);
        //        allToAdd.addAll(mutated);
        //        allToAdd.addAll(keep);


        //        for(int net = 0; net < allToAdd.size(); net++){
        //            NetsForGeneration[net] = allToAdd.get(net);
        //        }

        //    }

        //        private ArrayList<NeuralNetwork> getNewNetworks()
        //        {
        //            int numToGen = (int)(Population * 0.1);
        //            ArrayList<NeuralNetwork> newNets = new ArrayList<NeuralNetwork>();
        //            for (int i = 0; i < numToGen; i++)
        //            {
        //                NeuralNetwork newNet = new NeuralNetwork(5, 4, 1); // CAREFUL THIS IS HARD CODED!
        //                newNets.add(newNet);
        //            }
        //            return newNets;
        //        }

        //        private ArrayList<NeuralNetwork> keep(int[] indicesToKeep)
        //        {
        //            ArrayList<NeuralNetwork> toKeep = new ArrayList<NeuralNetwork>();
        //            for (int i = 0; i < indicesToKeep.length; i++)
        //            {
        //                NeuralNetwork goodPerformer = NetsForGeneration[indicesToKeep[i]];
        //                toKeep.add(goodPerformer);
        //            }
        //            return toKeep;
        //        }

        //        private ArrayList<NeuralNetwork> mutate(int[] indicesToKeep)
        //        {
        //            int numToMutate = (int)(Population * 0.1);
        //            // chance of mutation is 5% for now
        //            int numMutated = 0;
        //            ArrayList<NeuralNetwork> mutated = new ArrayList<NeuralNetwork>();
        //            Random random = new Random();
        //            while (numMutated < numToMutate)
        //            {
        //                int i = random.nextInt(indicesToKeep.length);
        //                NeuralNetwork goodPerformer = NetsForGeneration[indicesToKeep[i]];
        //                ArrayList<ArrayList<Double[]>> genes = goodPerformer.getWeightMatrix();
        //                ArrayList<ArrayList<Double[]>> childGenes = new ArrayList<ArrayList<Double[]>>();

        //                for (int layer = 0; layer < genes.size(); layer++)
        //                {
        //                    ArrayList<Double[]> motherLayer = genes.get(layer);
        //                    ArrayList<Double[]> childLayer = new ArrayList<Double[]>();
        //                    for (int n = 0; n < motherLayer.size(); n++)
        //                    {
        //                        Double[] motherNeuronWeights = motherLayer.get(n);
        //                        Double[] childNeuronWeights = new Double[motherNeuronWeights.length];

        //                        for (int weightIndex = 0; weightIndex < childNeuronWeights.length; weightIndex++)
        //                        {
        //                            if (new Random().NextDouble() > MUTATE_CHANCE)
        //                            {
        //                                childNeuronWeights[weightIndex] = motherNeuronWeights[weightIndex];
        //                            }
        //                            else
        //                            {
        //                                double val = new Random().NextDouble();
        //                                if (new Random().NextDouble() < 0.5)
        //                                {
        //                                    // 50% chance of being negative, being between -1 and 1
        //                                    val = 0 - val;
        //                                }
        //                                childNeuronWeights[weightIndex] = val;
        //                            }
        //                        }
        //                        childLayer.add(childNeuronWeights);
        //                    }
        //                    childGenes.add(childLayer);
        //                }

        //                NeuralNetwork child = new NeuralNetwork(5, 4, 1); // CAREFUL THESE ARE HARDCODED!
        //                child.setWeightMatrix(childGenes);
        //                mutated.add(child);
        //                numMutated++;
        //            }
        //            return mutated;
        //        }

        //        private ArrayList<NeuralNetwork> breed(int[] indicesToKeep)
        //        {
        //            int numToBreed = (int)(Population * 0.3);
        //            double sumOfAllEvals = 0;
        //            for (int i = 0; i < indicesToKeep.length; i++)
        //            {
        //                sumOfAllEvals += Evals[indicesToKeep[i]];
        //            }

        //            ArrayList<NeuralNetwork> children = new ArrayList<NeuralNetwork>();
        //            for (int bred = 0; bred < numToBreed; bred++)
        //            {
        //                ArrayList<WeightedIndex> toChooseFrom = new ArrayList<WeightedIndex>();
        //                for (int i = 0; i < indicesToKeep.length; i++)
        //                {
        //                    double value = Evals[indicesToKeep[i]];
        //                    double weight = value / sumOfAllEvals;
        //                    WeightedIndex index = new WeightedIndex(indicesToKeep[i], weight);
        //                    toChooseFrom.add(index);
        //                }

        //                // choose mother
        //                WeightedIndex index1 = chooseIndex(toChooseFrom);
        //                toChooseFrom.remove(index1);
        //                NeuralNetwork mother = NetsForGeneration[index1.index];

        //                // choose father
        //                WeightedIndex index2 = chooseIndex(toChooseFrom);
        //                toChooseFrom.remove(index2);
        //                NeuralNetwork father = NetsForGeneration[index2.index];

        //                NeuralNetwork child = mate(mother, father);
        //                children.add(child);
        //            }

        //            return children;

        //        }

        //        private NeuralNetwork mate(NeuralNetwork mother, NeuralNetwork father)
        //        {
        //            ArrayList<ArrayList<Double[]>> motherGenes = mother.getWeightMatrix();
        //            ArrayList<ArrayList<Double[]>> fatherGenes = father.getWeightMatrix();
        //            ArrayList<ArrayList<Double[]>> childGenes = new ArrayList<ArrayList<Double[]>>();
        //            for (int layer = 0; layer < motherGenes.size(); layer++)
        //            {
        //                ArrayList<Double[]> motherLayer = motherGenes.get(layer);
        //                ArrayList<Double[]> fatherLayer = fatherGenes.get(layer);
        //                ArrayList<Double[]> childLayer = new ArrayList<Double[]>();
        //                for (int n = 0; n < motherLayer.size(); n++)
        //                {
        //                    Double[] motherNeuronWeights = motherLayer.get(n);
        //                    Double[] fatherNeuronWeights = fatherLayer.get(n);
        //                    Double[] childNeuronWeights = new Double[motherNeuronWeights.length];

        //                    for (int i = 0; i < childNeuronWeights.length; i++)
        //                    {
        //                        if (new Random().NextDouble() > 0.5)
        //                        {
        //                            childNeuronWeights[i] = motherNeuronWeights[i];
        //                        }
        //                        else
        //                        {
        //                            childNeuronWeights[i] = fatherNeuronWeights[i];
        //                        }
        //                    }
        //                    childLayer.add(childNeuronWeights);
        //                }
        //                childGenes.add(childLayer);
        //            }
        //            NeuralNetwork child = new NeuralNetwork(5, 4, 1); // CAREFUL THESE ARE HARDCODED!
        //            child.setWeightMatrix(childGenes);
        //            return child;
        //        }

        //        private WeightedIndex chooseIndex(ArrayList<WeightedIndex> indices) {
        //        while (true) {
        //            for (WeightedIndex index : indices) {
        //                double random = new Random().NextDouble();
        //                if (random > (1 - index.weight)) {
        //                    return index;
        //                }
        //            }
        //        }

        //    }

        //        public bool generationFinished()
        //        {
        //            var isDone = false;
        //            lock (CompletedThisGeneration)
        //            {
        //                if (CompletedThisGeneration == Population)
        //                {
        //                    CompletedThisGeneration = 0;
        //                    isDone = true;
        //                }
        //            }

        //            return isDone;
        //        }

        //        public static void main(String[] args)
        //        {
        //            GeneticAlgorithm evolver = new GeneticAlgorithm(500);
        //            evolver.runEpoch();

        //            // evolver.runGeneration();
        //            // while(!evolver.generationFinished()){
        //            // try {
        //            // Thread.sleep(500);
        //            // } catch (InterruptedException e) {
        //            // // TODO Auto-generated catch block
        //            // e.printStackTrace();
        //            // }
        //            // }
        //            // int count = 0;
        //            // int[] results = evolver.getEvals();
        //            // for(int i = 0; i < results.length; i++){
        //            // count++;
        //            // System.out.println("eval: " + results[i]);
        //            // }
        //            // System.out.println("count: " + count);
        //        }
    }

}
