using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class WeightedSession
    {
        public ITrainingSession Session;
        public double Weight;
        public double CumlativeWeight;

    }
}
