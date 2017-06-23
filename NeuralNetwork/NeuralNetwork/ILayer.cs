using ArtificialNeuralNetwork.Genes;
using System.Collections.Generic;

namespace ArtificialNeuralNetwork
{
    public interface ILayer
    {
        void Process();
        LayerGene GetGenes();
        IList<INeuron> NeuronsInLayer { get; set; }
    }
}
