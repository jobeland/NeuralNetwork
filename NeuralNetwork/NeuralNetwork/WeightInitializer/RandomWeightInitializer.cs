using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.WeightInitializer
{
    public class RandomWeightInitializer : IWeightInitializer
    {
        private readonly Random _random;

        public RandomWeightInitializer(Random random)
        {
            _random = new Random();
        }

        public double InitializeWeight()
        {
            double val = _random.NextDouble();
            if (_random.NextDouble() < 0.5)
            {
                // 50% chance of being negative, being between -1 and 1
                val = 0 - val;
            }
            return val;
        }
    }
}
