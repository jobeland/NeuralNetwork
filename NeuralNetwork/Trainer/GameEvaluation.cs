using ArtificialNeuralNetwork;
using BasicGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedTraining;
using UnsupervisedTraining.Evaluatable;

namespace Trainer
{
    public class GameEvaluation : IEvaluatable
    {
        private readonly Game _game;
        private readonly INeuralNetwork _neuralNet;

        public GameEvaluation(Game game, INeuralNetwork neuralNet)
        {
            _game = game;
            _neuralNet = neuralNet;
        }

        public void RunEvaluation()
        {
            Array values = Enum.GetValues(typeof(MoveDirection));
            while (!_game.IsGameWon() && !_game.IsGameLost())
            {
                MoveDirection dirToMove = MoveDirection.DOWN;
                double highestProb = double.MinValue;
                foreach (MoveDirection val in values)
                {
                    double distance = _game.GetDistanceToClosestDot(val, _game.CurrentCoord, new List<Tuple<int, int>>());
                    _neuralNet.SetInputs(new[] { distance });
                    _neuralNet.Process();
                    double probability = _neuralNet.GetOutputs()[0];
                    if (probability > highestProb)
                    {
                        dirToMove = val;
                        highestProb = probability;
                    }
                }
                _game.UseTurn(dirToMove);
            }
        }

        public double GetEvaluation()
        {
            if (!_game.IsGameWon() && !_game.IsGameLost())
            {
                throw new NotSupportedException("GetSessionEvaluation is not supported when game is not finished");
            }
            return _game.MovesLeft + (_game.Width * _game.Length) - _game.GetDotsLeft();
        }
    }
}
