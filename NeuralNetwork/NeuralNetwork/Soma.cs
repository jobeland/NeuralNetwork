using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    public class Soma : ISoma
    {
        private readonly IList<Synapse> _dendrites;
        private readonly ISummationFunction _summationFunction;

        public Soma(IList<Synapse> dendrites, ISummationFunction summationFunction)
        {
            _dendrites = dendrites;
            _summationFunction = summationFunction;
        }

        public double CalculateSummation()
        {
            return _summationFunction.CalculateSummation(_dendrites);
        }
    }
}
