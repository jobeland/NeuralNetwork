using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface IAxon
    {
        void ProcessSignal(double signal);
        double Value { get; }
        AxonGene GetGenes();
    }
}
