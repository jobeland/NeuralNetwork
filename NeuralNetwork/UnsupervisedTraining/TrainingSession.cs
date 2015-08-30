using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork;
using BasicGame;
using Logging;

namespace UnsupervisedTraining
{

    public class TrainingSession : ITrainingSession
    {
        private int _sessionNumber;
        public INeuralNetwork NeuralNet { get; set; }
        private Game _game;
        private bool _hasStoredSessionEval;
        private double _sessionEval;

        public TrainingSession(INeuralNetwork nn, Game game, int sessionNumber)
        {
            NeuralNet = nn;
            _game = game;
            _sessionNumber = sessionNumber;
            _hasStoredSessionEval = false;
            _sessionEval = 0;
        }

        public void Run()
        {
            LoggerFactory.GetLogger().Log(LogLevel.Debug, string.Format("Starting training session {0}",_sessionNumber));
            Array values = Enum.GetValues(typeof(MoveDirection));

                Game g = new Game(10, 10, 300);
                //while (!_game.IsGameWon() && !_game.IsGameLost())
                //{
                //    MoveDirection dirToMove = MoveDirection.DOWN;
                    
                //    double[] inputs = new double[4];
                //    int dir = 0;
                //    MoveDirection[] directions = new MoveDirection[4];
                //    foreach (MoveDirection val in values)
                //    {
                //        double distance = _game.GetDistanceToClosestDot(val, _game.CurrentCoord, new List<Tuple<int, int>>());
                //        inputs[dir] = distance;
                //        directions[dir] = val;
                //        dir++;
                //    }
                //    _nn.setInputs(inputs);
                //    _nn.CalculateActivation();
                //    double[] probabilities = _nn.GetOutput();
                //    double highestProb = double.MinValue;
                //    for (int i = 0; i < probabilities.Length; i++)
                //    {
                //        if (probabilities[i] > highestProb)
                //        {
                //            dirToMove = directions[i];
                //            highestProb = probabilities[i];
                //        }
                //    }
                        
                //    _game.UseTurn(dirToMove);
                //}
                while (!_game.IsGameWon() && !_game.IsGameLost())
                {
                    MoveDirection dirToMove = MoveDirection.DOWN;
                    double highestProb = double.MinValue;
                    foreach (MoveDirection val in values)
                    {
                        double distance = _game.GetDistanceToClosestDot(val, _game.CurrentCoord, new List<Tuple<int, int>>());
                        NeuralNet.SetInputs(new[] { distance });
                        NeuralNet.Process();
                        double probability = NeuralNet.GetOutputs()[0];
                        if (probability > highestProb)
                        {
                            dirToMove = val;
                            highestProb = probability;
                        }
                    }
                    _game.UseTurn(dirToMove);
                }
                LoggerFactory.GetLogger().Log(LogLevel.Debug, string.Format("Stopping training session {0}", _sessionNumber));
        }

        public double GetSessionEvaluation()
        {
            if (!_game.IsGameWon() && !_game.IsGameLost())
            {
                throw new NotSupportedException("GetSessionEvaluation is not supported when game is not finished");
            }
            if (_hasStoredSessionEval)
            {
                return _sessionEval;
            }
            else
            {
                _sessionEval = _game.MovesLeft + (_game.Width * _game.Length) - _game.GetDotsLeft();
                _hasStoredSessionEval = true;
            }
            return _sessionEval;
        }



    }
}
