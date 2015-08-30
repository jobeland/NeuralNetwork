using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace UnsupervisedTraining
{
    public interface IBreeder
    {
        IList<INeuralNetwork> Breed(IList<TrainingSession> sessions, int numToBreed);
    }
}
