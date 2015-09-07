using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace UnsupervisedTraining.Evolution
{
    public interface IMutator
    {
        IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance);
    }
}
