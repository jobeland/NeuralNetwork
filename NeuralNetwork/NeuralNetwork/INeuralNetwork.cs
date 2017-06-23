using ArtificialNeuralNetwork.Genes;

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
