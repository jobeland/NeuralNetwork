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

        protected double output { get; set; }
        protected double input { get; set; }

        public Neuron()
        {
            output = 0;
            input = 0;
        }

        protected double calculateActivationFunction()
        {
            return input;
        }

        protected void fire()
        {
            this.output = calculateActivationFunction();
            this.input = 0;
        }

    }
}
