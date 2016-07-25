using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.SummationFunctions
{
    public class MaxSummation : ISummationFunction
    {
        public double CalculateSummation(IList<Synapse> dendrites, double bias)
        {
            double max = bias;
            foreach (Synapse synapse in dendrites)
            {
                var weightedValue = synapse.Axon.Value * synapse.Weight;
                if (weightedValue > max)
                {
                    max = weightedValue;
                }
            }
            return max;
        }
    }
}
