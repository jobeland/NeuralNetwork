using ArtificialNeuralNetwork.WeightInitializer;

namespace ArtificialNeuralNetwork.Factories
{
    public class SynapseFactory : ISynapseFactory
    {
        private IWeightInitializer _weightInitializer;

        private SynapseFactory(IWeightInitializer weightInitializer)
        {
            _weightInitializer = weightInitializer;
        }

        public static ISynapseFactory GetInstance(IWeightInitializer weightInitializer)
        {
            return new SynapseFactory(weightInitializer);
        }

        public Synapse Create()
        {
            return new Synapse
            {
                Weight = _weightInitializer.InitializeWeight()
            };
        }

        public Synapse Create(double weight)
        {
            return new Synapse
            {
                Weight = weight
            };
        }
    }
}
