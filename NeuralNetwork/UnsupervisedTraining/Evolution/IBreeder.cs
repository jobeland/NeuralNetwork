using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace UnsupervisedTraining.Evolution
{
    public interface IBreeder
    {
        IList<INeuralNetwork> Breed(IList<ITrainingSession> sessions, int numToBreed);
    }
}
