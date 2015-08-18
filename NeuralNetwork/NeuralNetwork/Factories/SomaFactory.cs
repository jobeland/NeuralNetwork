using ArtificialNeuralNetwork.SummationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public class SomaFactory : ISomaFactory
    {
        private ISummationFunction _summationFunction;

        private SomaFactory(ISummationFunction summationFunction)
        {
            _summationFunction = summationFunction;
        }

        public static ISomaFactory GetInstance(ISummationFunction summationFunction)
        {
            return new SomaFactory(summationFunction);
        }

        public ISoma Create(IList<Synapse> dendrites, double bias)
        {
            return Soma.GetInstance(dendrites, _summationFunction, bias);
        }

        public ISoma Create(IList<Synapse> dendrites, double bias, Type summationFunction)
        {
            if (summationFunction == typeof(AverageSummation))
            {
                return Soma.GetInstance(dendrites, new AverageSummation(), bias);
            }
            else if (summationFunction == typeof(MaxSummation))
            {
                return Soma.GetInstance(dendrites, new MaxSummation(), bias);
            }
            else if (summationFunction == typeof(MinSummation))
            {
                return Soma.GetInstance(dendrites, new MinSummation(), bias);
            }
            else if (summationFunction == typeof(SimpleSummation))
            {
                return Soma.GetInstance(dendrites, new SimpleSummation(), bias);
            }
            else
            {
                throw new NotSupportedException(string.Format("{0} is not a supported summation function type for Create()", summationFunction));
            }
        }
    }
}
