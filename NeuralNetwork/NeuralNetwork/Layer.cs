using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{

    [Serializable]
    public class Layer : ILayer
    {
        private readonly IList<INeuron> _neuronsInLayer;

        private Layer(IList<INeuron> neuronsInLayer)
        {
            _neuronsInLayer = neuronsInLayer;
        }

        public static ILayer GetInstance(IList<INeuron> neuronsInLayer)
        {
            return new Layer(neuronsInLayer);
        }

        public void Process()
        {
            foreach (INeuron n in _neuronsInLayer)
            {
                n.Process();
            }
        }
    }

}
