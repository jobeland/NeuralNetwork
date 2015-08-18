using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    public class SimpleSummation : ISummationFunction
    {
        public double CalculateSummation(IList<Synapse> dendrites, double bias)
        {
            double sum = bias;
            foreach (Synapse synapse in dendrites)
            {
                sum += synapse.Value * synapse.Weight;
            }
            return sum;
        }
    }
}
