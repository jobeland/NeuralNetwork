using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
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

        public virtual double CalculateActivationFunction()
        {
            return Input;
        }

        public virtual void Fire()
        {
            this.Output = CalculateActivationFunction();
            this.Input = 0;
        }

    }
}
