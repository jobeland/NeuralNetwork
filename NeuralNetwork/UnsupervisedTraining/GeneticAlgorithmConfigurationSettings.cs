using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class GeneticAlgorithmConfigurationSettings
    {
        public bool UseMultithreading { get; set; }
        public double NormalMutationRate { get; set; }
        public double HighMutationRate { get; set; }
        public int GenerationPopulation { get; set; }
        public int GenerationsPerEpoch { get; set; }
        public int NumEpochs { get; set; }
    }
}
