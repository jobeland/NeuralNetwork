using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedTraining;

namespace Trainer
{
    class Runner
    {
        static void Main(string[] args)
        {
            GeneticAlgorithm evolver = new GeneticAlgorithm(500);
            evolver.runEpoch();
        }
    }
}
