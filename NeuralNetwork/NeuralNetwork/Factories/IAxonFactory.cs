using System;
using System.Collections.Generic;
namespace ArtificialNeuralNetwork.Factories
{
    public interface IAxonFactory
    {
        IAxon Create(IList<Synapse> terminals);
        IAxon Create(IList<Synapse> terminals, Type activationFunction);
    }
}
