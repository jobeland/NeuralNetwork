using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    public class ConstantWeightInitializer : IWeightInitializer
    {
        private readonly double _value;

        public ConstantWeightInitializer(double constantValue)
        {
            _value = constantValue;
        }

        public double InitializeWeight()
        {
            return _value;
        }
    }
}
