using System;
namespace UnsupervisedTraining
{
    public interface IEvalWorkingSet
    {
        void AddEval(double eval);
        bool IsStale();
    }
}
