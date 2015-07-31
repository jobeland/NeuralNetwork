using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.ActivationFunctions
{
    public class TanhActivationFunction : ActivationFunction
    {
        public double Calculate(double sumOfInputsAndBias)
        {
            return Math.Tanh(sumOfInputsAndBias);
        }
    }
}
