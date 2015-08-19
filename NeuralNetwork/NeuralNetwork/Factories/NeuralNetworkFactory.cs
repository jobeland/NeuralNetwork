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
        //TODO: this class needs to create and setup the entire network
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

        public INeuralNetwork Create(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            var somaFactory = SomaFactory.GetInstance(_summationFunction);
            var axonFactory = AxonFactory.GetInstance(_activationFunction);

            //layer number + position in layer --> list of terminals
            var mapping = CreateSynapses(numInputs, numOutputs, numHiddenLayers, numHiddenPerLayer);

            //Input Layer
            IList<Synapse> inputs = new List<Synapse>();
            IList<INeuron> inputLayerNeurons = new List<INeuron>();
            for (int i = 0; i < numInputs; i++)
            {
                var dendrites = mapping[0][i];
                foreach (var dendrite in dendrites)
                {
                    inputs.Add(dendrite);
                }
                var soma = somaFactory.Create(dendrites, _weightInitializer.InitializeWeight());

                var terminals = mapping[1][i];
                var axon = axonFactory.Create(terminals);

                inputLayerNeurons.Add(Neuron.GetInstance(soma, axon));
            }
            ILayer inputLayer = Layer.GetInstance(inputLayerNeurons);


            IList<ILayer> hiddenLayers = new List<ILayer>();
            //Hidden layers 0 to (n-1)
            for (int h = 0; h < numHiddenLayers -1; h++)
            {
                IList<INeuron> hiddenNeuronsInLayer = new List<INeuron>();
                for (int i = 0; i < numHiddenPerLayer; i++)
                {
                    var dendrites = getDendritesForSoma(h+2, i, mapping);

                    var soma = somaFactory.Create(dendrites, _weightInitializer.InitializeWeight());

                    var terminals = mapping[h + 2][i];
                    var axon = axonFactory.Create(terminals);
                    hiddenNeuronsInLayer.Add(Neuron.GetInstance(soma, axon));
                }
                hiddenLayers.Add(Layer.GetInstance(hiddenNeuronsInLayer));
            }

            //Hidden layer n
            IList<INeuron> hiddenNeuronsInLastHiddenLayer = new List<INeuron>();
                for (int i = 0; i < numHiddenPerLayer; i++)
                {
                    var dendrites = getDendritesForSoma(numHiddenLayers + 1, i, mapping);

                    var soma = somaFactory.Create(dendrites, _weightInitializer.InitializeWeight());

                    var terminals = mapping[numHiddenLayers + 1][i];
                    var axon = axonFactory.Create(terminals);
                    hiddenNeuronsInLastHiddenLayer.Add(Neuron.GetInstance(soma, axon));
                }
                hiddenLayers.Add(Layer.GetInstance(hiddenNeuronsInLastHiddenLayer));


            //Output layer
            IList<Synapse> outputs = new List<Synapse>();
            IList<INeuron> outputLayerNeurons = new List<INeuron>();
            for (int o = 0; o < numOutputs;o++)
            {
                var dendrites = getDendritesForSoma(numHiddenLayers + 2, o, mapping);
                var soma = somaFactory.Create(dendrites, _weightInitializer.InitializeWeight());

                var terminals = mapping[numHiddenLayers + 2][o];
                var axon = axonFactory.Create(terminals);
                outputLayerNeurons.Add(Neuron.GetInstance(soma, axon));
                foreach (var dendrite in terminals)
                {
                    outputs.Add(dendrite);
                }
            }
            ILayer outputLayer = Layer.GetInstance(outputLayerNeurons);

            return NeuralNetwork.GetInstance(inputs, inputLayer, hiddenLayers, outputLayer, outputs);
        }

        internal Dictionary<int, Dictionary<int, IList<Synapse>>> CreateSynapses(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer)
        {
            var synapseFactory = SynapseFactory.GetInstance(_weightInitializer);
            //layer number + position in layer --> list of terminals
            var mapping = new Dictionary<int, Dictionary<int, IList<Synapse>>>();

            //Input Layer
            IList<Synapse> inputs = new List<Synapse>();
            mapping[0] = new Dictionary<int, IList<Synapse>>();
            mapping[1] = new Dictionary<int, IList<Synapse>>();
            for (int i = 0; i < numInputs; i++)
            {
                var synapse = synapseFactory.Create();

                var dendrites = new[] { synapse };
                mapping[0][i] = dendrites;

                var terminals = new List<Synapse>();
                for (int t = 0; t < numHiddenPerLayer; t++)
                {
                    terminals.Add(synapseFactory.Create());
                }
                mapping[1][i] = terminals;
            }

            //Hidden layers 0 to (n-1)
            for (int h = 0; h < numHiddenLayers - 1; h++)
            {
                mapping[h + 2] = new Dictionary<int, IList<Synapse>>();
                for (int i = 0; i < numHiddenPerLayer; i++)
                {
                    var terminals = new List<Synapse>();
                    for (int t = 0; t < numHiddenPerLayer; t++)
                    {
                        terminals.Add(synapseFactory.Create());
                    }
                    mapping[h + 2][i] = terminals;
                }
            }

            //Hidden layer n
            mapping[numHiddenLayers + 1] = new Dictionary<int, IList<Synapse>>();
            for (int i = 0; i < numHiddenPerLayer; i++)
            {
                var terminals = new List<Synapse>();
                for (int t = 0; t < numOutputs; t++)
                {
                    terminals.Add(synapseFactory.Create());
                }
                mapping[numHiddenLayers + 1][i] = terminals;
            }

            //Output layer
            mapping[numHiddenLayers + 2] = new Dictionary<int, IList<Synapse>>();
            for (int o = 0; o < numOutputs; o++)
            {
                var synapse = synapseFactory.Create();
                var terminals = new List<Synapse>();
                for (int t = 0; t < numOutputs; t++)
                {
                    terminals.Add(synapseFactory.Create());
                }
                mapping[numHiddenLayers + 2][o] = terminals;
            }

            return mapping;
        }

        private IList<Synapse> getDendritesForSoma(int layer, int neuronIndexInLayer, Dictionary<int, Dictionary<int, IList<Synapse>>> mapping)
        {
            //get entire layer before, then grab the nth synapse from each list
            if(layer <= 0){
                throw new ArgumentOutOfRangeException("layer must be > 0");
            }
            
            var neuronMappings = mapping[layer-1];
            IList<Synapse> dendrites = new List<Synapse>();
            foreach(var key in neuronMappings.Keys){
                dendrites.Add(neuronMappings[key][neuronIndexInLayer]);
            }

            return dendrites;
        }


        public INeuralNetwork Create(Genes.NeuralNetworkGene genes)
        {
            throw new NotImplementedException();
        }
    }
}
