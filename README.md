# NeuralNetwork
A .Net implementation of an artificial neural network

An example of evolving this neural network using a genetic algorithm can be found at [GeneticAlgorithm](https://github.com/jobeland/GeneticAlgorithm) and a full example using the neural network and genetic algorithm to evolve it's topology for a specific task can be found at [BasicGameNeuralNetworkTrainer](https://github.com/jobeland/BasicGameNeuralNetworkTrainer)


### Creating a Neural Network

Creating an instance of NeuralNetwork can be done using an instance of NeuralNetworkFactory which implements INeuralNetworkFactory.

##### The Short Way
The short way to create one is to use the default values as follows:
```c#
var numInputs = 3;
var numOutputs = 1;
var numHiddenLayers = 1;
var numNeuronsInHiddenLayer = 5;
INeuralNetwork network = NeuralNetworkFactory.GetInstance().Create(numInputs, numOutputs, numHiddenLayers, numNeuronsInHiddenLayer);
```

This will create an instance of INeuralNetwork which has 3 inputs, 1 output, and 1 hidden layer containing 5 neurons. 

##### The Long Way
If you wish to override some of the inner functionality, you can do so by extending the dependent interface factories and injecting them. Below are the default values that are set the same as if you used the short way, just explicitly injected:
```c#
var somaFactory = SomaFactory.GetInstance(new SimpleSummation());
var axonFactory = AxonFactory.GetInstance(new TanhActivationFunction());
var randomInit = new RandomWeightInitializer(new Random());
var hiddenSynapseFactory = SynapseFactory.GetInstance(randomInit);
var ioSynapseFactory = SynapseFactory.GetInstance(new ConstantWeightInitializer(1.0));
var biasInitializer = randomInit;
INeuralNetworkFactory factory = NeuralNetworkFactory.GetInstance(somaFactory, axonFactory, hiddenSynapseFactory, ioSynapseFactory, biasInitializer);

var numInputs = 3;
var numOutputs = 1;
var numHiddenLayers = 1;
var numNeuronsInHiddenLayer = 5;
INeuralNetwork network = factory.Create(numInputs, numOutputs, numHiddenLayers, numNeuronsInHiddenLayer);
```

### Using the Network
Using the network requires setting the inputs, calling `Process()` on the network, and the getting the outputs of the network:
```c#
var inputs = new double[] { 1.4, 2.04045, 4.2049558 };
network.SetInputs(inputs);
network.Process();
var outputs = newtwork.GetOutputs();
```

### Configuring the Topology
When evolving the network, you can get and set the topology of the network
```c#
NeuralNetworkGene genes = network.GetGenes();
...
modify gene code
...
INeuralNetwork newNetwork = factory.Create(genes);
```


