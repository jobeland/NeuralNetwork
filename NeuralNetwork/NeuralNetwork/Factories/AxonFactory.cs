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

        public IAxon Create(IList<Synapse> terminals, Type activationFunction)
        {
            if (activationFunction == typeof(AbsoluteXActivationFunction))
            {
                return Axon.GetInstance(terminals, new AbsoluteXActivationFunction());
            }
            else if (activationFunction == typeof(IdentityActivationFunction))
            {
                return Axon.GetInstance(terminals, new IdentityActivationFunction());
            }
            else if (activationFunction == typeof(InverseActivationFunction))
            {
                return Axon.GetInstance(terminals, new InverseActivationFunction());
            }
            else if (activationFunction == typeof(SechActivationFunction))
            {
                return Axon.GetInstance(terminals, new SechActivationFunction());
            }
            else if (activationFunction == typeof(SigmoidActivationFunction))
            {
                return Axon.GetInstance(terminals, new SigmoidActivationFunction());
            }
            else if (activationFunction == typeof(SinhActivationFunction))
            {
                return Axon.GetInstance(terminals, new SinhActivationFunction());
            }
            else if (activationFunction == typeof(StepActivationFunction))
            {
                return Axon.GetInstance(terminals, new StepActivationFunction());
            }
            else if (activationFunction == typeof(TanhActivationFunction))
            {
                return Axon.GetInstance(terminals, new TanhActivationFunction());
            }
            else
            {
                throw new NotSupportedException(string.Format("{0} is not a supported activation function type for Create()", activationFunction));
            }
        }
    }
}
