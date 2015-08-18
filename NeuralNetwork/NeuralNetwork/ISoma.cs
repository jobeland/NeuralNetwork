using ArtificialNeuralNetwork.Genes;
using System;
namespace ArtificialNeuralNetwork
{
    public interface ISoma
    {
        double CalculateSummation();
        SomaGene GetGenes();
    }
}
