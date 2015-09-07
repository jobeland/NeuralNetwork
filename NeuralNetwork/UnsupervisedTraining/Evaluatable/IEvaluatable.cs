using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining.Evaluatable
{
    public interface IEvaluatable
    {
        void RunEvaluation();
        double GetEvaluation();
    }
}
