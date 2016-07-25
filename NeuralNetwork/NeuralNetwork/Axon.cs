using ArtificialNeuralNetwork.ActivationFunctions;
using ArtificialNeuralNetwork.Genes;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    public class Axon : IAxon
    {
        private readonly IList<Synapse> _terminals;
        private readonly IActivationFunction _activationFunction;
        public double Value { get; private set; }

        private Axon(IList<Synapse> terminals, IActivationFunction activationFunction)
        {
            _activationFunction = activationFunction;
            _terminals = terminals;
            Value = 0.0;
            foreach (var synapse in terminals)
            {
                synapse.Axon = this;
            }
        }

        public static IAxon GetInstance(IList<Synapse> terminals, IActivationFunction activationFunction)
        {
            return new Axon(terminals, activationFunction);
        }

        public void ProcessSignal(double signal)
        {
            Value = calculateActivation(signal);
        }

        internal double calculateActivation(double signal)
        {
            return _activationFunction.CalculateActivation(signal);
        }

        public AxonGene GetGenes()
        {
            return new AxonGene
            {
                ActivationFunction = _activationFunction.GetType(),
                Weights = _terminals.Select(d => d.Weight).ToList()
            };
        }

    }
}
