using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining.Evolution
{
    public class EvolutionConfigurationSettings
    {
        public double NormalMutationRate { get; set; }
        public double HighMutationRate { get; set; }
        public int GenerationsPerEpoch { get; set; }
        public int NumEpochs { get; set; }
    }
}
