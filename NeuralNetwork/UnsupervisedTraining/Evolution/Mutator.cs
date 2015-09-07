using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Factories;
using ArtificialNeuralNetwork.Genes;
using ArtificialNeuralNetwork.SummationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining.Evolution
{
    public class Mutator : IMutator
    {
        private readonly INeuralNetworkFactory _networkFactory;
        private readonly IWeightInitializer _weightInitializer;
        private readonly MutationConfigurationSettings _config;

        public Mutator(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer, MutationConfigurationSettings config)
        {
            _networkFactory = networkFactory;
            _weightInitializer = weightInitializer;
            _config = config;
        }

        public IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance)
        {
            List<INeuralNetwork> completed = new List<INeuralNetwork>();
            Random random = new Random();
            foreach (INeuralNetwork net in networks)
            {
                NeuralNetworkGene childGenes = net.GetGenes();
                if (_config.MutateNumberOfHiddenLayers)
                {
                    childGenes = TryAddLayerToNetwork(childGenes, mutateChance, random);
                }

                for (int n = 0; n < childGenes.InputGene.Neurons.Count; n++)
                {
                    var neuron = childGenes.InputGene.Neurons[n];
                    childGenes.InputGene.Neurons[n] = TryMutateNeuron(neuron, random, mutateChance);
                }

                for (int h = 0; h < childGenes.HiddenGenes.Count; h++)
                {
                    if (_config.MutateNumberOfHiddenNeuronsInLayer)
                    {
                        childGenes.HiddenGenes[h] = TryAddNeuronsToLayer(childGenes, h, mutateChance, random);
                    }

                    for (int j = 0; j < childGenes.HiddenGenes[h].Neurons.Count; j++)
                    {
                        var neuron = childGenes.HiddenGenes[h].Neurons[j];
                        childGenes.HiddenGenes[h].Neurons[j] = TryMutateNeuron(neuron, random, mutateChance);
                    }
                }
                completed.Add(_networkFactory.Create(childGenes));
            }
            return completed;
        }

        internal NeuralNetworkGene TryAddLayerToNetwork(NeuralNetworkGene genes, double mutateChance, Random random)
        {
            NeuralNetworkGene newGenes = genes;
            while (random.NextDouble() <= mutateChance)
            {
                int layerToReplace = random.Next(newGenes.HiddenGenes.Count);

                //update layer-1 axon terminals
                LayerGene previousLayer = GetPreviousLayerGene(newGenes, layerToReplace);
                foreach (NeuronGene neuron in previousLayer.Neurons)
                {
                    neuron.Axon.Weights.Clear();
                    neuron.Axon.Weights.Add(_weightInitializer.InitializeWeight());
                }

                LayerGene newLayer = new LayerGene
                {
                    Neurons = new List<NeuronGene>()
                };
                newGenes.HiddenGenes.Insert(layerToReplace, newLayer);

                var newNeuron = GetRandomHiddenNeuronGene(newGenes, layerToReplace, random);
                newGenes.HiddenGenes[layerToReplace].Neurons.Add(newNeuron);
            }
            return newGenes;
        }

        internal LayerGene TryAddNeuronsToLayer(NeuralNetworkGene networkGenes, int hiddenLayerIndex, double mutateChance, Random random)
        {
            LayerGene hiddenLayer = networkGenes.HiddenGenes[hiddenLayerIndex];
            while (random.NextDouble() <= mutateChance)
            {
                //update layer-1 axon terminals
                LayerGene previousLayer = GetPreviousLayerGene(networkGenes, hiddenLayerIndex);
                foreach (NeuronGene neuron in previousLayer.Neurons)
                {
                    neuron.Axon.Weights.Add(_weightInitializer.InitializeWeight());
                }

                hiddenLayer.Neurons.Add(GetRandomHiddenNeuronGene(networkGenes, hiddenLayerIndex, random));
            }
            return hiddenLayer;
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
            //weights
            for (int j = 0; j < gene.Axon.Weights.Count; j++)
            {
                if (_config.MutateSynapseWeights && random.NextDouble() <= mutateChance)
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


            //bias
            if (_config.MutateSomaBiasFunction && random.NextDouble() <= mutateChance)
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

            //activation
            if (_config.MutateAxonActivationFunction && random.NextDouble() <= mutateChance)
            {
                toReturn.Axon.ActivationFunction = GetRandomActivationFunction(random).GetType();
            }
            else
            {
                toReturn.Axon.ActivationFunction = gene.Axon.ActivationFunction;
            }

            //summation
            if (_config.MutateSomaSummationFunction && random.NextDouble() <= mutateChance)
            {
                toReturn.Soma.SummationFunction = GetRandomSummationFunction(random).GetType();
            }
            else
            {
                toReturn.Soma.SummationFunction = gene.Soma.SummationFunction;
            }
            return gene;
        }

        internal LayerGene GetPreviousLayerGene(NeuralNetworkGene genes, int hiddenLayerIndex)
        {
            if (hiddenLayerIndex == 0)
            {
                return genes.InputGene;
            }
            else
            {
                return genes.HiddenGenes[hiddenLayerIndex - 1];
            }
        }

        internal LayerGene GetNextLayerGene(NeuralNetworkGene genes, int hiddenLayerIndex)
        {
            if (hiddenLayerIndex == genes.HiddenGenes.Count - 1)
            {
                return genes.OutputGene;
            }
            else
            {
                return genes.HiddenGenes[hiddenLayerIndex + 1];
            }
        }

        internal NeuronGene GetRandomHiddenNeuronGene(NeuralNetworkGene networkGenes, int hiddenLayerIndex, Random random)
        {
            var neuronGene = new NeuronGene
            {
                Axon = new AxonGene
                {
                    Weights = new List<double>(),
                    ActivationFunction = GetRandomActivationFunction(random).GetType()
                },
                Soma = new SomaGene
                {
                    Bias = _weightInitializer.InitializeWeight(),
                    SummationFunction = GetRandomSummationFunction(random).GetType()
                }
            };
            //update terminals for current neuron
            LayerGene nextlayer = GetNextLayerGene(networkGenes, hiddenLayerIndex);
            for (int i = 0; i < nextlayer.Neurons.Count; i++)
            {
                neuronGene.Axon.Weights.Add(_weightInitializer.InitializeWeight());
            }
            return neuronGene;
        }

        internal IActivationFunction GetRandomActivationFunction(Random random)
        {
            var value = random.Next(8);
            switch (value)
            {
                case 0:
                    return new TanhActivationFunction();
                case 1:
                    return new StepActivationFunction();
                case 2:
                    return new SinhActivationFunction();
                case 3:
                    return new AbsoluteXActivationFunction();
                case 4:
                    return new SechActivationFunction();
                case 5:
                    return new InverseActivationFunction();
                case 6:
                    return new IdentityActivationFunction();
                case 7:
                default:
                    return new SigmoidActivationFunction();
            }
        }

        internal ISummationFunction GetRandomSummationFunction(Random random)
        {
            var value = random.Next(4);
            switch (value)
            {
                case 0:
                    return new MinSummation();
                case 1:
                    return new AverageSummation();
                case 2:
                    return new MaxSummation();
                case 3:
                default:
                    return new SimpleSummation();
            }
        }
    }
}
