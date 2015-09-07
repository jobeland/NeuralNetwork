using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining.Utils
{
    public class EvalWorkingSet : IEvalWorkingSet
    {
        private LinkedList<Double> _pastEvals;
        private int _size;

        public EvalWorkingSet(int size)
        {
            _pastEvals = new LinkedList<Double>();
            _pastEvals.AddFirst(0.0);
            this._size = size;
        }

        public void AddEval(double eval)
        {
            _pastEvals.AddFirst(eval);
            if (_pastEvals.Count > this._size)
            {
                _pastEvals.RemoveLast();
            }
        }

        public bool IsStale()
        {
            if (_pastEvals.First.Value <= _pastEvals.Last.Value)
            {
                return true;
            }
            return false;
        }

    }
}
