using ArtificialNeuralNetwork.SummationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Genes
{
    public class SomaGene
    {
        public IList<double> Weights;
        public double Bias;
        public SupportedSummationFunctions SummationFunction;
    }
}
