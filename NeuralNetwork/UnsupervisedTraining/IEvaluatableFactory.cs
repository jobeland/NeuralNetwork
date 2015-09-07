using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public interface IEvaluatableFactory
    {
        IEvaluatable Create(INeuralNetwork neuralNetwork);
    }
}
