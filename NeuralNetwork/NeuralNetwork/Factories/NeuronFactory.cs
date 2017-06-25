namespace ArtificialNeuralNetwork.Factories
{
    public class NeuronFactory : INeuronFactory
    {
        public static INeuronFactory GetInstance()
        {
            return new NeuronFactory();
        }

        public INeuron Create(ISoma soma, IAxon axon)
        {
            return Neuron.GetInstance(soma, axon);
        }
    }
}
