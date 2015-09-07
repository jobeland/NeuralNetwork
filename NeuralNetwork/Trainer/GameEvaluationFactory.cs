using ArtificialNeuralNetwork;
using BasicGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedTraining;

namespace Trainer
{
    public class GameEvaluationFactory : IEvaluatableFactory
    {
        public IEvaluatable Create(INeuralNetwork neuralNetwork)
        {
            return new GameEvaluation(new Game(10, 10, 300), neuralNetwork);
        }
    }
}
