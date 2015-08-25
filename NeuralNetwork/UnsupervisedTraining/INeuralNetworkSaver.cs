using System;
namespace UnsupervisedTraining
{
    interface INeuralNetworkSaver
    {
        string SaveNeuralNetwork(ArtificialNeuralNetwork.INeuralNetwork network, double networkEvaluation, int epoch);
    }
}
