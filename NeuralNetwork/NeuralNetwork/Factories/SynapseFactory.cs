using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public class SynapseFactory
    {
        private IWeightInitializer _weightInitializer;

        private SynapseFactory(IWeightInitializer weightInitializer)
        {
            _weightInitializer = weightInitializer;
        }

        public static SynapseFactory GetInstance(IWeightInitializer weightInitializer)
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
    }
}
