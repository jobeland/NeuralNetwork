using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public class NeuralNetworkFactory : INeuralNetworkFactory
    {
        private ISummationFunction _summationFunction;
        private IActivationFunction _activationFunction;
        private IWeightInitializer _weightInitializer;

        private NeuralNetworkFactory(ISummationFunction summationFunction, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
        {
            _summationFunction = summationFunction;
            _activationFunction = activationFunction;
            _weightInitializer = weightInitializer;
        }

        public static NeuralNetworkFactory GetInstance(ISummationFunction summationFunction, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
        {
            return new NeuralNetworkFactory(summationFunction, activationFunction, weightInitializer);
        }

        internal INeuron CreateNeuron(ISomaFactory somaFactory, IAxonFactory axonFactory, Dictionary<int, Dictionary<int, IList<Synapse>>> mapping, int layerIndex, int neuronIndex)
        {
            var dendrites = (layerIndex > 0) ? getDendritesForSoma(layerIndex, neuronIndex, mapping) : mapping[layerIndex][neuronIndex];

            var soma = somaFactory.Create(dendrites, _weightInitializer.InitializeWeight());

            var terminals = mapping[layerIndex + 1][neuronIndex];
            var axon = axonFactory.Create(terminals);

            return Neuron.GetInstance(soma, axon);
        }

        //Used for input/output lists
        internal IList<Synapse> GetAllSynapsesFromLayerMapping(Dictionary<int, IList<Synapse>> layerMapping)
        {
            IList<Synapse> synapses = new List<Synapse>();
            foreach(var key in layerMapping.Keys)
            {
                var terminals = layerMapping[key];
                foreach (var terminal in terminals)
                {
                    synapses.Add(terminal);
                }
            }
            return synapses;
        }

        internal ILayer CreateLayer(ISomaFactory somaFactory, IAxonFactory axonFactory, Dictionary<int, Dictionary<int, IList<Synapse>>> synapseMapping, int layerInNetwork, int numberOfNeurons)
        {
            IList<INeuron> layerNeurons = new List<INeuron>();
            for (int i = 0; i < numberOfNeurons; i++)
            {
                layerNeurons.Add(CreateNeuron(somaFactory, axonFactory, synapseMapping, layerInNetwork, i));
            }
            return Layer.GetInstance(layerNeurons);
        }

        public INeuralNetwork Create(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            var somaFactory = SomaFactory.GetInstance(_summationFunction);
            var axonFactory = AxonFactory.GetInstance(_activationFunction);

            //layer number + position in layer --> list of terminals
            var mapping = CreateSynapses(numInputs, numOutputs, numHiddenLayers, numHiddenPerLayer);

            var inputs = GetAllSynapsesFromLayerMapping(mapping[0]);
            var outputs = GetAllSynapsesFromLayerMapping(mapping[mapping.Keys.Count - 1]);

            ILayer inputLayer = CreateLayer(somaFactory, axonFactory, mapping, 0, numInputs);

            //Hidden layers
            IList<ILayer> hiddenLayers = new List<ILayer>();
            for (int h = 0; h < numHiddenLayers; h++)
            {
                hiddenLayers.Add(CreateLayer(somaFactory, axonFactory, mapping, h + 1, numHiddenPerLayer));
            }

            ILayer outputLayer = CreateLayer(somaFactory, axonFactory, mapping, numHiddenLayers + 1, numOutputs);

            return NeuralNetwork.GetInstance(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        internal Dictionary<int, Dictionary<int, IList<Synapse>>> CreateSynapses(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            var synapseFactory = SynapseFactory.GetInstance(_weightInitializer);
            //layer number + position in layer --> list of terminals
            var mapping = new Dictionary<int, Dictionary<int, IList<Synapse>>>();

            //Input synapses
            mapping[0] = CreateSynapseMapLayer(synapseFactory, numInputs, 1);

            //Input neuron terminals
            mapping[1] = CreateSynapseMapLayer(synapseFactory, numInputs, numHiddenPerLayer);

            //Hidden layers 0 to (n-1)
            for (int h = 0; h < numHiddenLayers - 1; h++)
            {
                mapping[h + 2] = CreateSynapseMapLayer(synapseFactory, numHiddenPerLayer, numHiddenPerLayer);
            }

            //Hidden layer n
            mapping[numHiddenLayers + 1] = CreateSynapseMapLayer(synapseFactory, numHiddenPerLayer, numOutputs);

            //Output layer
            mapping[numHiddenLayers + 2] = CreateSynapseMapLayer(synapseFactory, numOutputs, 1);

            return mapping;
        }

        internal Dictionary<int, IList<Synapse>> CreateSynapseMapLayer(ISynapseFactory synapseFactory, int numberOfNeuronsInLayer, int numberOfTerminalsPerNeuron)
        {
            var mapLayer = new Dictionary<int, IList<Synapse>>();
            for (int i = 0; i < numberOfNeuronsInLayer; i++)
            {
                mapLayer[i] = CreateTerminals(synapseFactory, numberOfTerminalsPerNeuron);
            }
            return mapLayer;
        }

        internal IList<Synapse> CreateTerminals(ISynapseFactory synapseFactory, int numberOfSynapses)
        {
            var terminals = new List<Synapse>();
            for (int t = 0; t < numberOfSynapses; t++)
            {
                terminals.Add(synapseFactory.Create());
            }
            return terminals;
        }

        private IList<Synapse> getDendritesForSoma(int layer, int terminalIndexInLayer, Dictionary<int, Dictionary<int, IList<Synapse>>> mapping)
        {
            //get entire layer before, then grab the nth synapse from each list
            if (layer <= 1)
            {
                throw new ArgumentOutOfRangeException("layer must be > 1");
            }

            var neuronMappings = mapping[layer];
            IList<Synapse> dendrites = new List<Synapse>();
            foreach (var neuron in neuronMappings.Keys)
            {
                dendrites.Add(neuronMappings[neuron][terminalIndexInLayer]);
            }

            return dendrites;
        }


        public INeuralNetwork Create(Genes.NeuralNetworkGene genes)
        {
            throw new NotImplementedException();
        }
    }
}
