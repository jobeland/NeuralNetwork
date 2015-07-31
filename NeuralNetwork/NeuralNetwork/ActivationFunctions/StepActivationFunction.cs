﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.ActivationFunctions
{
    public class StepActivationFunction : IActivationFunction
    {
        public double Calculate(double sumOfInputsAndBias)
        {
            if (sumOfInputsAndBias >= 0)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }
    }
}
