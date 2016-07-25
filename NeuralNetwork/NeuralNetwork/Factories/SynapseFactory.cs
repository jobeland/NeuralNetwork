using System.Collections.Generic;
using ArtificialNeuralNetwork.WeightInitializer;

namespace ArtificialNeuralNetwork.Factories
{
    public class SynapseFactory : ISynapseFactory
    {
        private IWeightInitializer _weightInitializer;
        private IAxonFactory _axonFactory;

        private SynapseFactory(IWeightInitializer weightInitializer, IAxonFactory axonFactory)
        {
            _weightInitializer = weightInitializer;
            _axonFactory = axonFactory;
        }

        public static ISynapseFactory GetInstance(IWeightInitializer weightInitializer, IAxonFactory axonFactory)
        {
            return new SynapseFactory(weightInitializer, axonFactory);
        }

        public Synapse Create()
        {
            return new Synapse
            {
                Axon = _axonFactory.Create(),
                Weight = _weightInitializer.InitializeWeight()
            };
        }

        public Synapse Create(double weight)
        {
            return new Synapse
            {
                Axon = _axonFactory.Create(),
                Weight = weight
            };
        }
    }
}
