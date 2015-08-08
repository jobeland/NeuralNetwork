using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public class SomaFactory
    {
        private ISummationFunction _summationFunction;

        private SomaFactory(ISummationFunction summationFunction)
        {
            _summationFunction = summationFunction;
        }

        public static SomaFactory GetInstance(ISummationFunction summationFunction)
        {
            return new SomaFactory(summationFunction);
        }

        public ISoma Create(IList<Synapse> dendrites)
        {
            return Soma.GetInstance(dendrites, _summationFunction);
        }
    }
}
