using System.Collections.Generic;
using ArtificialNeuralNetwork.Genes;

namespace ArtificialNeuralNetwork
{
    public interface INeuralNetwork
    {
        ILayer InputLayer { get; set; }
        IList<ILayer> HiddenLayers { get; set; }
        ILayer OutputLayer { get; set; }
        IList<Synapse> Inputs { get; set; }
        IList<Synapse> Outputs { get; set; }
        double[] GetOutputs();
        void Process();
        void SetInputs(double[] inputs);
        NeuralNetworkGene GetGenes();
    }
}
