using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Genes
{
    public class AxonGene
    {
        public Type ActivationFunction;
        public IList<double> Weights;
    }
}
