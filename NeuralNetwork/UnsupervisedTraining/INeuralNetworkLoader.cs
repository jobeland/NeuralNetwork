using ArtificialNeuralNetwork;
using System;
namespace UnsupervisedTraining
{
    interface INeuralNetworkLoader
    {
        INeuralNetwork LoadNeuralNetwork(string filename);
    }
}
