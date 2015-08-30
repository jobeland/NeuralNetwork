using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class WeightedSessionList
    {
        private IList<WeightedSession> _sessions;

        public WeightedSessionList(IList<TrainingSession> sessions)
        {
            double sumOfAllEvals = 0;
            for (int i = 0; i < sessions.Count; i++)
            {
                sumOfAllEvals += sessions[i].GetSessionEvaluation();
            }
            if (sumOfAllEvals <= 0)
            {
                sumOfAllEvals = 1;
            }

            List<WeightedSession> toChooseFrom = new List<WeightedSession>();
            double cumulative = 0.0;
            for (int i = 0; i < sessions.Count; i++)
            {
                //TODO: this weight determination algorithm should be delegated
                double value = sessions[i].GetSessionEvaluation();
                double weight = value / sumOfAllEvals;
                WeightedSession weightedSession = new WeightedSession
                {
                    Session = sessions[i],
                    Weight = weight,
                };
                toChooseFrom.Add(weightedSession);
            }

            toChooseFrom = toChooseFrom.OrderBy(session => session.Weight).ToList();
            foreach (WeightedSession session in toChooseFrom)
            {
                session.CumlativeWeight = cumulative;
                cumulative += session.Weight;
            }
            _sessions = toChooseFrom;
        }

        public TrainingSession ChooseRandomWeightedSession()
        {
            double value = RandomGenerator.GetInstance().NextDouble() * _sessions[_sessions.Count - 1].CumlativeWeight;
            //Failsafe for odd case when value is very low. Needs a more permanent fix so as not to skew the selection towards lower, however slight           
            if (_sessions[0].CumlativeWeight > value)
            {
                return _sessions[0].Session;
            }
            return _sessions.Last(session => session.CumlativeWeight <= value).Session;
        }
    }
}
