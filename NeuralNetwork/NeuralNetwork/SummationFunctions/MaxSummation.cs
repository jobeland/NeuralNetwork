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
                if (synapse.Value > max)
                {
                    max = synapse.Value;
                }
            }
            return max;
        }
    }
}
