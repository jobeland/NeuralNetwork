using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface INeuron
    {
        void Process();
        NeuronGene GetGenes();
    }
}
