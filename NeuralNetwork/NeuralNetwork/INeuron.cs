using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface INeuron
    {
        ISoma Soma { get; set; }
        IAxon Axon { get; set; }
        void Process();
        NeuronGene GetGenes();
    }
}
