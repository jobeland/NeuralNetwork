using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
namespace UnsupervisedTraining
{
    public interface IMutator
    {
        IList<INeuralNetwork> Mutate(IList<INeuralNetwork> networks, double mutateChance);
    }
}
