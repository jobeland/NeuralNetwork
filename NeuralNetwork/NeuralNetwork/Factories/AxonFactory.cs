using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public class AxonFactory : IAxonFactory
    {
        private IActivationFunction _activationFunction;

        private AxonFactory(IActivationFunction activationFunction)
        {
            _activationFunction = activationFunction;
        }

        public static IAxonFactory GetInstance(IActivationFunction activationFunction)
        {
            return new AxonFactory(activationFunction);
        }

        public IAxon Create(IList<Synapse> terminals)
        {
            return Axon.GetInstance(terminals, _activationFunction);
        }
    }
}
