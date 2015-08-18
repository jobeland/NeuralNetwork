using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface ILayer
    {
        void Process();
        LayerGene GetGenes();
    }
}
