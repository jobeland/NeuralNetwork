using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class Mutator : IMutator
    {
        private readonly INeuralNetworkFactory _networkFactory;

        public Mutator(INeuralNetworkFactory networkFactory)
        {
            _networkFactory = networkFactory;
        }

        public IList<INeuralNetwork> Mutate(IList<TrainingSession> sessions, int numToMutate, double mutateChance)
        {
            int numMutated = 0;
            List<INeuralNetwork> mutated = new List<INeuralNetwork>();
            Random random = new Random();
            while (numMutated < numToMutate)
            {
                int i = random.Next(sessions.Count);
                INeuralNetwork goodPerformer = sessions[i].NeuralNet;
                NeuralNetworkGene childGenes = goodPerformer.GetGenes();

                for (int n = 0; n < childGenes.InputGene.Neurons.Count; n++)
                {
                    var neuron = childGenes.InputGene.Neurons[n];
                    childGenes.InputGene.Neurons[n] = TryMutateNeuron(neuron, random, mutateChance);
                }

                for (int h = 0; h < childGenes.HiddenGenes.Count; h++)
                {
                    for (int j = 0; j < childGenes.HiddenGenes[h].Neurons.Count; j++)
                    {
                        var neuron = childGenes.HiddenGenes[h].Neurons[j];
                        childGenes.HiddenGenes[h].Neurons[j] = TryMutateNeuron(neuron, random, mutateChance);
                    }
                }
                mutated.Add(_networkFactory.Create(childGenes));
                numMutated++;
            }
            return mutated;
        }

        internal NeuronGene TryMutateNeuron(NeuronGene gene, Random random, double mutateChance)
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
                if (random.NextDouble() <= mutateChance)
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
            if (random.NextDouble() <= mutateChance)
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
    }
}
