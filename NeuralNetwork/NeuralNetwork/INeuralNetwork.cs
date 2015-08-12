using System;
namespace ArtificialNeuralNetwork
{
    public interface INeuralNetwork
    {
        double[] GetOutputs();
        void Process();
        void SetInputs(double[] inputs);
    }
}
