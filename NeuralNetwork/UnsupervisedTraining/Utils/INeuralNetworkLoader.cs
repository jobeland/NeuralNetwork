using ArtificialNeuralNetwork;
using System;
namespace UnsupervisedTraining.Utils
{
    interface INeuralNetworkLoader
    {
        INeuralNetwork LoadNeuralNetwork(string filename);
    }
}
