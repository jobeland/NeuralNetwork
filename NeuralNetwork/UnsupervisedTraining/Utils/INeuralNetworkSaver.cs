using System;
namespace UnsupervisedTraining.Utils
{
    interface INeuralNetworkSaver
    {
        string SaveNeuralNetwork(ArtificialNeuralNetwork.INeuralNetwork network, double networkEvaluation, int epoch);
    }
}
