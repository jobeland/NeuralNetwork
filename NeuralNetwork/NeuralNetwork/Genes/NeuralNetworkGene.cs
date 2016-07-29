using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.Genes
{
    public class NeuralNetworkGene
    {
        public LayerGene InputGene;
        public IList<LayerGene> HiddenGenes;
        public LayerGene OutputGene;
    }
}
