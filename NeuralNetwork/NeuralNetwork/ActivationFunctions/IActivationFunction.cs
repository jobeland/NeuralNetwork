using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.ActivationFunctions
{
    public interface IActivationFunction
    {
        double Calculate(double sumOfInputsAndBias);
    }
}
