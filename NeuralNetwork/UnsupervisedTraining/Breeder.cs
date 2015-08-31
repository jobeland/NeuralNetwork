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
    public class Breeder : IBreeder
    {
        private readonly INeuralNetworkFactory _networkFactory;
        private const double MOTHER_FATHER_BIAS = 0.5;


        public Breeder(INeuralNetworkFactory networkFactory)
        {
            _networkFactory = networkFactory;
        }

        public IList<INeuralNetwork> Breed(IList<ITrainingSession> sessions, int numToBreed)
        {
            WeightedSessionList weightedSessions = new WeightedSessionList(sessions);
            List<INeuralNetwork> children = new List<INeuralNetwork>();
            for (int bred = 0; bred < numToBreed; bred++)
            {
                // choose mother
                ITrainingSession session1 = weightedSessions.ChooseRandomWeightedSession();
                INeuralNetwork mother = session1.NeuralNet;

                // choose father
                ITrainingSession session2 = weightedSessions.ChooseRandomWeightedSession();
                INeuralNetwork father = session2.NeuralNet;

                INeuralNetwork child = mate(mother, father);
                children.Add(child);
            }

            return children;

        }

        internal INeuralNetwork mate(INeuralNetwork mother, INeuralNetwork father)
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

            if (childFatherGenes.HiddenGenes.Count >= motherGenes.HiddenGenes.Count)
            {
                childFatherGenes.HiddenGenes = MateHiddenLayers(childFatherGenes.HiddenGenes, motherGenes.HiddenGenes, random);
            }
            else
            {
                childFatherGenes.HiddenGenes = MateHiddenLayers(motherGenes.HiddenGenes, childFatherGenes.HiddenGenes, random);
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

        internal IList<LayerGene> MateHiddenLayers(IList<LayerGene> moreLayers, IList<LayerGene> lessLayers, Random random)
        {
            var matedLayers = new List<LayerGene>();
            for (int h = 0; h < moreLayers.Count; h++)
            {
                //check to make sure they both have that hidden layer and only breed that layer if they both do. otherwise just keep the father's
                if (h < lessLayers.Count)
                {
                    if (moreLayers[h].Neurons.Count >= lessLayers[h].Neurons.Count)
                    {
                        matedLayers.Add(MateLayer(moreLayers[h], lessLayers[h], random));
                    }
                    else
                    {
                        matedLayers.Add(MateLayer(lessLayers[h], moreLayers[h], random));
                    }
                }
                else
                {
                    matedLayers.Add(moreLayers[h]);
                }
            }
            return matedLayers;
        }


        internal LayerGene MateLayer(LayerGene moreNeurons, LayerGene lessNeurons, Random random)
        {
            LayerGene childGene = new LayerGene
            {
                Neurons = new List<NeuronGene>()
            };

            for (int j = 0; j < moreNeurons.Neurons.Count; j++)
            {
                //only breed the neuron if the mother also has it. Otherwise just leave add the extra neuron untouched.
                if (j < lessNeurons.Neurons.Count)
                {
                    var neuron = moreNeurons.Neurons[j];
                    var lessNeuron = lessNeurons.Neurons[j];
                    childGene.Neurons.Add(BreedNeuron(neuron, lessNeuron, random));
                }
                else
                {
                    childGene.Neurons.Add(moreNeurons.Neurons[j]);
                }
            }
            return childGene;
        }

        internal IList<double> MateAxonWeights(NeuronGene moreTerminals, NeuronGene lessTerminals, Random random)
        {
            var weights = new List<double>();
            for (int j = 0; j < moreTerminals.Axon.Weights.Count; j++)
            {
                if (random.NextDouble() < MOTHER_FATHER_BIAS && j < lessTerminals.Axon.Weights.Count)
                {
                    weights.Add(lessTerminals.Axon.Weights[j]);
                }
                else
                {
                    weights.Add(moreTerminals.Axon.Weights[j]);
                }
            }
            return weights;
        }


        internal NeuronGene BreedNeuron(NeuronGene father, NeuronGene mother, Random random)
        {
            NeuronGene toReturn = new NeuronGene
            {
                Axon = new AxonGene(),
                Soma = new SomaGene()
            };
            if (father.Axon.Weights.Count >= mother.Axon.Weights.Count)
            {
                toReturn.Axon.Weights = MateAxonWeights(father, mother, random);
            }
            else
            {
                toReturn.Axon.Weights = MateAxonWeights(mother, father, random);
            }

            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Axon.ActivationFunction = mother.Axon.ActivationFunction;
            }
            else
            {
                toReturn.Axon.ActivationFunction = father.Axon.ActivationFunction;
            }

            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Soma.SummationFunction = mother.Soma.SummationFunction;
            }
            else
            {
                toReturn.Soma.SummationFunction = father.Soma.SummationFunction;
            }

            if (random.NextDouble() < MOTHER_FATHER_BIAS)
            {
                toReturn.Soma.Bias = mother.Soma.Bias;
            }
            else
            {
                toReturn.Soma.Bias = father.Soma.Bias;
            }

            return toReturn;
        }
    }   
}
