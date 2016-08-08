using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Genes;
using ArtificialNeuralNetwork.WeightInitializer;
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
        private readonly ISomaFactory _somaFactory;
        private readonly IAxonFactory _axonFactory;
        private readonly ISynapseFactory _hiddenSynapseFactory;
        private readonly ISynapseFactory _inputOutputSynapseFactory;
        private readonly IWeightInitializer _biasInitiliazer;

        private NeuralNetworkFactory(ISomaFactory somaFactory, IAxonFactory axonFactory, ISynapseFactory hiddenSynapseFactory, ISynapseFactory inputOutputSynapseFactory, IWeightInitializer biasInitializer)
        {
            _somaFactory = somaFactory;
            _axonFactory = axonFactory;
            _hiddenSynapseFactory = hiddenSynapseFactory;
            _inputOutputSynapseFactory = inputOutputSynapseFactory;
            _biasInitiliazer = biasInitializer;
        }

        public static NeuralNetworkFactory GetInstance(ISomaFactory somaFactory, IAxonFactory axonFactory, ISynapseFactory hiddenSynapseFactory, ISynapseFactory inputOutputSynapseFactory, IWeightInitializer biasInitializer)
        {
            return new NeuralNetworkFactory(somaFactory, axonFactory, hiddenSynapseFactory, inputOutputSynapseFactory, biasInitializer);
        }

        public static NeuralNetworkFactory GetInstance()
        {
            var somaFactory = SomaFactory.GetInstance(new SimpleSummation());
            var axonFactory = AxonFactory.GetInstance(new TanhActivationFunction());
            var random = new Random();
            var randomInit = new RandomWeightInitializer(random);
            var synapseFactory = SynapseFactory.GetInstance(randomInit, axonFactory);
            var ioSynapseFactory = SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0), AxonFactory.GetInstance(new IdentityActivationFunction()));
            return new NeuralNetworkFactory(somaFactory, axonFactory, synapseFactory, ioSynapseFactory, randomInit);
        }

        internal INeuron CreateNeuron(Dictionary<int, Dictionary<int, IList<Synapse>>> mapping, int layerIndex, int neuronIndex)
        {
            var dendrites = (layerIndex > 0) ? getDendritesForSoma(layerIndex, neuronIndex, mapping) : mapping[layerIndex][neuronIndex];

            var soma = _somaFactory.Create(dendrites, _biasInitiliazer.InitializeWeight());

            var terminals = mapping[layerIndex + 1][neuronIndex];
            var axon = _axonFactory.Create(terminals);

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

        internal ILayer CreateLayer(Dictionary<int, Dictionary<int, IList<Synapse>>> synapseMapping, int layerInNetwork, int numberOfNeurons)
        {
            IList<INeuron> layerNeurons = new List<INeuron>();
            for (int i = 0; i < numberOfNeurons; i++)
            {
                layerNeurons.Add(CreateNeuron(synapseMapping, layerInNetwork, i));
            }
            return Layer.GetInstance(layerNeurons);
        }

        public INeuralNetwork Create(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            //layer number + position in layer --> list of terminals
            var mapping = CreateSynapses(numInputs, numOutputs, numHiddenLayers, numHiddenPerLayer);

            var inputs = GetAllSynapsesFromLayerMapping(mapping[0]);
            var outputs = GetAllSynapsesFromLayerMapping(mapping[mapping.Keys.Count - 1]);

            ILayer inputLayer = CreateLayer(mapping, 0, numInputs);

            //Hidden layers
            IList<ILayer> hiddenLayers = new List<ILayer>();
            for (int h = 0; h < numHiddenLayers; h++)
            {
                hiddenLayers.Add(CreateLayer(mapping, h + 1, numHiddenPerLayer));
            }

            ILayer outputLayer = CreateLayer(mapping, numHiddenLayers + 1, numOutputs);

            return NeuralNetwork.GetInstance(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        public INeuralNetwork Create(int numInputs, int numOutputs, IList<int> hiddenLayerSpecs)
        {
            //layer number + position in layer --> list of terminals
            var mapping = CreateSynapses(numInputs, numOutputs, hiddenLayerSpecs);

            var inputs = GetAllSynapsesFromLayerMapping(mapping[0]);
            var outputs = GetAllSynapsesFromLayerMapping(mapping[mapping.Keys.Count - 1]);

            ILayer inputLayer = CreateLayer(mapping, 0, numInputs);

            //Hidden layers
            IList<ILayer> hiddenLayers = new List<ILayer>();
            for (int h = 0; h < hiddenLayerSpecs.Count; h++)
            {
                hiddenLayers.Add(CreateLayer(mapping, h + 1, hiddenLayerSpecs[h]));
            }

            ILayer outputLayer = CreateLayer(mapping, hiddenLayerSpecs.Count + 1, numOutputs);

            return NeuralNetwork.GetInstance(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        internal Dictionary<int, Dictionary<int, IList<Synapse>>> CreateSynapses(int numInputs, int numOutputs, IList<int> hiddenLayerSpecs)
        {
            //layer number + position in layer --> list of terminals
            var mapping = new Dictionary<int, Dictionary<int, IList<Synapse>>>();

            //Input synapses
            mapping[0] = CreateSynapseMapLayer(_inputOutputSynapseFactory, numInputs, 1);

            //Input neuron terminals
            mapping[1] = CreateSynapseMapLayer(_hiddenSynapseFactory, numInputs, hiddenLayerSpecs[0]);

            //Hidden layers 0 to (n-1)
            for (var h = 0; h < hiddenLayerSpecs.Count - 1; h++)
            {
                mapping[h + 2] = CreateSynapseMapLayer(_hiddenSynapseFactory, hiddenLayerSpecs[h], hiddenLayerSpecs[h+1]);
            }

            //Hidden layer n
            mapping[hiddenLayerSpecs.Count + 1] = CreateSynapseMapLayer(_hiddenSynapseFactory, hiddenLayerSpecs[hiddenLayerSpecs.Count - 1], numOutputs);

            //Output layer
            mapping[hiddenLayerSpecs.Count + 2] = CreateSynapseMapLayer(_inputOutputSynapseFactory, numOutputs, 1);

            return mapping;
        }

        internal Dictionary<int, Dictionary<int, IList<Synapse>>> CreateSynapses(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            var hiddenSpecs = new List<int>();
            for (var i = 0; i < numHiddenLayers; i++)
            {
                hiddenSpecs.Add(numHiddenPerLayer);
            }
            return CreateSynapses(numInputs, numOutputs, hiddenSpecs);
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

        private IList<Synapse> getDendritesForSoma(int layer, int terminalIndexInNeuron, Dictionary<int, Dictionary<int, IList<Synapse>>> mapping)
        {
            //get entire layer before, then grab the nth synapse from each list
            if (layer < 1)
            {
                throw new ArgumentOutOfRangeException("layer must be > 0");
            }

            var neuronMappings = mapping[layer];
            IList<Synapse> dendrites = new List<Synapse>();
            foreach (var neuron in neuronMappings.Keys)
            {
                dendrites.Add(neuronMappings[neuron][terminalIndexInNeuron]);
            }

            return dendrites;
        }


        public INeuralNetwork Create(NeuralNetworkGene genes)
        {
            var mapping = CreateSynapsesFromGenes(genes);

            var inputs = GetAllSynapsesFromLayerMapping(mapping[0]);
            var outputs = GetAllSynapsesFromLayerMapping(mapping[mapping.Keys.Count - 1]);

            ILayer inputLayer = CreateLayerFromGene(genes.InputGene, mapping, 0);

            //Hidden layers
            IList<ILayer> hiddenLayers = new List<ILayer>();
            for (int h = 0; h < genes.HiddenGenes.Count; h++)
            {
                hiddenLayers.Add(CreateLayerFromGene(genes.HiddenGenes[h], mapping, h + 1));
            }

            ILayer outputLayer = CreateLayerFromGene(genes.OutputGene, mapping, genes.HiddenGenes.Count + 1);

            return NeuralNetwork.GetInstance(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }


        internal Dictionary<int, Dictionary<int, IList<Synapse>>> CreateSynapsesFromGenes(NeuralNetworkGene genes)
        {
            //layer number + position in layer --> list of terminals
            var mapping = new Dictionary<int, Dictionary<int, IList<Synapse>>>();

            //Input synapses
            mapping[0] = CreateSynapseMapLayer(_inputOutputSynapseFactory, genes.InputGene.Neurons.Count, 1);

            //Input neuron terminals
            mapping[1] = CreateSynapseMapLayerFromLayerGene(_inputOutputSynapseFactory, genes.InputGene);

            //Hidden layers
            for (int h = 0; h < genes.HiddenGenes.Count; h++)
            {
                mapping[h + 2] = CreateSynapseMapLayerFromLayerGene(_hiddenSynapseFactory, genes.HiddenGenes[h]);
            }
           
            //Output layer
            mapping[genes.HiddenGenes.Count + 2] = CreateSynapseMapLayer(_hiddenSynapseFactory, genes.OutputGene.Neurons.Count, 1);

            return mapping;
        }

        internal Dictionary<int, IList<Synapse>> CreateSynapseMapLayerFromLayerGene(ISynapseFactory synapseFactory, LayerGene layerGene)
        {
            var mapLayer = new Dictionary<int, IList<Synapse>>();
            for (int i = 0; i < layerGene.Neurons.Count; i++)
            {
                mapLayer[i] = CreateTerminalsFromWeightList(synapseFactory, layerGene.Neurons[i].Axon.Weights);
            }
            return mapLayer;
        }

        internal IList<Synapse> CreateTerminalsFromWeightList(ISynapseFactory synapseFactory, IList<double> weights)
        {
            var terminals = new List<Synapse>();
            for (int t = 0; t < weights.Count; t++)
            {
                terminals.Add(synapseFactory.Create(weights[t]));
            }
            return terminals;
        }

        internal ILayer CreateLayerFromGene(LayerGene layerGene, Dictionary<int, Dictionary<int, IList<Synapse>>> synapseMapping, int layerInNetwork)
        {
            IList<INeuron> layerNeurons = new List<INeuron>();
            for (int i = 0; i < layerGene.Neurons.Count; i++)
            {
                layerNeurons.Add(CreateNeuronFromGene(layerGene.Neurons[i], synapseMapping, layerInNetwork, i));
            }
            return Layer.GetInstance(layerNeurons);
        }

        internal INeuron CreateNeuronFromGene(NeuronGene neuronGene, Dictionary<int, Dictionary<int, IList<Synapse>>> mapping, int layerIndex, int neuronIndex)
        {
            var dendrites = (layerIndex > 0) ? getDendritesForSoma(layerIndex, neuronIndex, mapping) : mapping[layerIndex][neuronIndex];

            var soma = _somaFactory.Create(dendrites, neuronGene.Soma.Bias, neuronGene.Soma.SummationFunction);

            var terminals = mapping[layerIndex + 1][neuronIndex];
            var axon = _axonFactory.Create(terminals, neuronGene.Axon.ActivationFunction);

            return Neuron.GetInstance(soma, axon);
        }
    }
}
