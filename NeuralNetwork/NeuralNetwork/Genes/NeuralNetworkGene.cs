using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Genes
{
    public class NeuralNetworkGene
    {
        public LayerGene InputGene;
        public IList<LayerGene> HiddenGenes;
        public LayerGene OutputGene;
    }
}
