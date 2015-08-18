using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface INeuralNetwork
    {
        double[] GetOutputs();
        void Process();
        void SetInputs(double[] inputs);
        NeuralNetworkGene GetGenes();
    }
}
