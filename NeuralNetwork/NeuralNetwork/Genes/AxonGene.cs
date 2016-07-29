using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.Genes
{
    public class AxonGene
    {
        public Type ActivationFunction { get; set; }
        public IList<double> Weights { get; set; }
    }
}
