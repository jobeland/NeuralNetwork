using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Neuron : INeuron
    {
        private readonly ISoma _soma;
        private readonly IAxon _axon;

        public Neuron(ISoma soma, IAxon axon)
        {
            _soma = soma;
            _axon = axon;
        }

        public virtual double CalculateActivationFunction()
        {
            return 0.0;
        }

        public void Process()
        {
            _axon.ProcessSignal(_soma.CalculateSummation());
        }
    }
}
