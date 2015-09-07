using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class MutationConfigurationSettings
    {
        public bool MutateNumberOfHiddenLayers { get; set; }
        public bool MutateNumberOfHiddenNeuronsInLayer { get; set; }
        public bool MutateSynapseWeights { get; set; }
        public bool MutateAxonActivationFunction { get; set; }
        public bool MutateSomaSummationFunction { get; set; }
        public bool MutateSomaBiasFunction { get; set; }
    }
}
