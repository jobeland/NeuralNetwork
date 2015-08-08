using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.SummationFunctions
{
    public class AverageSummation : ISummationFunction
    {
        public double CalculateSummation(IList<Synapse> dendrites)
        {
            double average = 0.0;
            if(dendrites.Count == 0)
            {
                return 0.0;
            }
            foreach (Synapse synapse in dendrites)
            {
                average += synapse.Value
            }
            return average / dendrites.Count;
        }
    }
}
