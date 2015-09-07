using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork;
using Logging;
using UnsupervisedTraining.Evaluatable;

namespace UnsupervisedTraining
{

    public class TrainingSession : ITrainingSession
    {
        private int _sessionNumber;
        public INeuralNetwork NeuralNet { get; set; }
        private IEvaluatable _evaluatable;
        private bool _hasStoredSessionEval;
        private double _sessionEval;

        public TrainingSession(INeuralNetwork nn, IEvaluatable evaluatable, int sessionNumber)
        {
            NeuralNet = nn;
            _evaluatable = evaluatable;
            _sessionNumber = sessionNumber;
            _hasStoredSessionEval = false;
            _sessionEval = 0;
        }

        public void Run()
        {
            LoggerFactory.GetLogger().Log(LogLevel.Debug, string.Format("Starting training session {0}", _sessionNumber));
            _evaluatable.RunEvaluation();
            LoggerFactory.GetLogger().Log(LogLevel.Debug, string.Format("Stopping training session {0}", _sessionNumber));
        }

        public double GetSessionEvaluation()
        {
           
            if (_hasStoredSessionEval)
            {
                return _sessionEval;
            }
            else
            {
                _sessionEval = _evaluatable.GetEvaluation();
                _hasStoredSessionEval = true;
            }
            return _sessionEval;
        }



    }
}
