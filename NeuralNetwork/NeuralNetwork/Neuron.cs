using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    [Serializable]
    public class Neuron
    {

        public double Output { get; set; }
        public double Input { get; set; }

        public Neuron()
        {
            Output = 0;
            Input = 0;
        }

        protected double calculateActivationFunction()
        {
            return Input;
        }

        protected void fire()
        {
            this.Output = calculateActivationFunction();
            this.Input = 0;
        }

    }
}
