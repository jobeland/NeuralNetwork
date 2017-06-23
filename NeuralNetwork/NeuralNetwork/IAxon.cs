using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using ArtificialNeuralNetwork.ActivationFunctions;

namespace ArtificialNeuralNetwork
{
    public interface IAxon
    {
        IList<Synapse> Terminals { get; set; }
        IActivationFunction ActivationFunction { get; set; }
        void ProcessSignal(double signal);
        double Value { get; }
        AxonGene GetGenes();
    }
}
