using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.SummationFunctions
{
    public class MinSummation : ISummationFunction
    {
        public double CalculateSummation(IList<Synapse> dendrites)
        {
            double min = double.MaxValue;
            foreach (Synapse synapse in dendrites)
            {
                if (synapse.Value < min)
                {
                    min = synapse.Value;
                }
            }
            return min;
        }
    }
}
