using ArtificialNeuralNetwork.WeightInitializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Value = 0.0,
                Weight = _weightInitializer.InitializeWeight()
            };
        }

        public Synapse Create(double weight)
        {
            return new Synapse
            {
                Value = 0.0,
                Weight = weight
            };
        }
    }
}
