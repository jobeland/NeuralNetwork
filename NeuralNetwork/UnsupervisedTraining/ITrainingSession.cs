using ArtificialNeuralNetwork;
using System;
namespace UnsupervisedTraining
{
    public interface ITrainingSession
    {
        INeuralNetwork NeuralNet { get; }
        double GetSessionEvaluation();
        void Run();
    }
}
