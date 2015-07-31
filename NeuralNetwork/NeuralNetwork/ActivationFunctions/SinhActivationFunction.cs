using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.ActivationFunctions
{
    class SinhActivationFunction: IActivationFunction
    {
        public double Calculate(double sumOfInputsAndBias)
        {
            return Math.Sinh(sumOfInputsAndBias);
        }
    }
}
