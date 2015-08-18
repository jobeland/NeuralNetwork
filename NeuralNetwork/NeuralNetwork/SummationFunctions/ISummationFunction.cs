using System;
using System.Collections.Generic;
namespace ArtificialNeuralNetwork
{
    public interface ISummationFunction
    {
        double CalculateSummation(IList<Synapse> dendrites, double bias);
    }
}
