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

namespace UnsupervisedTraining
{
    public class Mutator : IMutator
    {
        private readonly INeuralNetworkFactory _networkFactory;
        private readonly IWeightInitializer _weightInitializer;

        public Mutator(INeuralNetworkFactory networkFactory, IWeightInitializer weightInitializer)
        {
            _networkFactory = networkFactory;
            _weightInitializer = weightInitializer;
        }

        public IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance)
        {
            List<INeuralNetwork> completed = new List<INeuralNetwork>();
            Random random = new Random();
            foreach (INeuralNetwork net in networks)
            {
                NeuralNetworkGene childGenes = net.GetGenes();

                for (int n = 0; n < childGenes.InputGene.Neurons.Count; n++)
                {
                    var neuron = childGenes.InputGene.Neurons[n];
                    childGenes.InputGene.Neurons[n] = TryMutateNeuron(neuron, random, mutateChance);
                }

                for (int h = 0; h < childGenes.HiddenGenes.Count; h++)
                {
                    childGenes.HiddenGenes[h] = TryAddNeuronsToLayer(childGenes, h, mutateChance, random);
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

        internal LayerGene TryAddNeuronsToLayer(NeuralNetworkGene networkGenes, int hiddenLayerIndex, double mutateChance, Random random)
        {
            LayerGene hiddenLayer = networkGenes.HiddenGenes[hiddenLayerIndex];
            while(random.NextDouble() <= mutateChance)
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

                //update layer-1 axon terminals
                LayerGene previousLayer = null;
                if (hiddenLayerIndex == 0)
                {
                    previousLayer = networkGenes.InputGene;
                }
                else
                {
                    previousLayer = networkGenes.HiddenGenes[hiddenLayerIndex - 1];
                }
                foreach (NeuronGene neuron in previousLayer.Neurons)
                {
                    neuron.Axon.Weights.Add(_weightInitializer.InitializeWeight());
                }

                //update terminals for current neuron
                LayerGene nextlayer = null;
                if (hiddenLayerIndex == networkGenes.HiddenGenes.Count - 1)
                {
                    nextlayer = networkGenes.OutputGene;
                }
                else
                {
                    nextlayer = networkGenes.HiddenGenes[hiddenLayerIndex + 1];
                }
                for (int i = 0; i < nextlayer.Neurons.Count; i++)
                {
                    neuronGene.Axon.Weights.Add(_weightInitializer.InitializeWeight());
                }

                hiddenLayer.Neurons.Add(neuronGene);
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

            //bias
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

            //activation
            if (random.NextDouble() <= mutateChance)
            {
                toReturn.Axon.ActivationFunction = GetRandomActivationFunction(random).GetType();
            }
            else
            {
                toReturn.Axon.ActivationFunction = gene.Axon.ActivationFunction;
            }

            //summation
            if (random.NextDouble() <= mutateChance)
            {
                toReturn.Soma.SummationFunction = GetRandomSummationFunction(random).GetType();
            }
            else
            {
                toReturn.Soma.SummationFunction = gene.Soma.SummationFunction;
            }
            return gene;
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
