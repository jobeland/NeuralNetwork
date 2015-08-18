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

        private Neuron(ISoma soma, IAxon axon)
        {
            _soma = soma;
            _axon = axon;
        }

        public static INeuron GetInstance(ISoma soma, IAxon axon)
        {
            return new Neuron(soma, axon);
        }

        public void Process()
        {
            _axon.ProcessSignal(_soma.CalculateSummation());
        }

        
    }
}
