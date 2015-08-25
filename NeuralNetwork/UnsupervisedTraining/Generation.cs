using ArtificialNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class Generation
    {
        public double[] Evals { get; set; }
        public IList<TrainingSession> _sessions { get; set; }
        private readonly GenerationConfigurationSettings _generationConfig;

        public Generation(IList<TrainingSession> population, GenerationConfigurationSettings generationConfig)
        {
            _sessions = population;
            _generationConfig = generationConfig;
        }

        public void Run()
        {
            if (_generationConfig.UseMultithreading)
            {
                Parallel.ForEach<TrainingSession>(_sessions, session =>
                {
                    session.Run();
                });
            }
            else
            {
                foreach (var session in _sessions)
                {
                    session.Run();
                }
            }
        }

        public double[] GetEvalsForGeneration()
        {
            if (Evals == null)
            {
                Evals = new double[_sessions.Count];
                for (int i = 0; i < _sessions.Count; i++)
                {
                    Evals[i] = _sessions[i].GetSessionEvaluation();
                }
            }
            //TODO: this shouldn't be in Evals anymore, but just called directly off of the training session
            double[] toReturn = new double[_sessions.Count];
            for (int i = 0; i < _sessions.Count; i++)
            {
                toReturn[i] = Evals[i];
            }
            return toReturn;
        }

        public TrainingSession GetBestPerformer()
        {
            int indexToKeep = 0;
            for (int performer = 0; performer < Evals.Length; performer++)
            {
                double value = Evals[performer];
                if (value > Evals[indexToKeep])
                {
                    indexToKeep = performer;
                }
            }
            return _sessions[indexToKeep];
        }
    }
}
