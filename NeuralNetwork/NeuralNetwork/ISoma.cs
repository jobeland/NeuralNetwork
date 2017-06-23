using ArtificialNeuralNetwork.Genes;
using System.Collections.Generic;

namespace ArtificialNeuralNetwork
{
    public interface ISoma
    {
        IList<Synapse> Dendrites { get; set; }
        ISummationFunction SummationFunction { get; set; }
        double Bias { get; set; }
        double CalculateSummation();
        SomaGene GetGenes();
    }
}
