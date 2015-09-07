using System;
namespace UnsupervisedTraining.Utils
{
    public interface IEvalWorkingSet
    {
        void AddEval(double eval);
        bool IsStale();
    }
}
