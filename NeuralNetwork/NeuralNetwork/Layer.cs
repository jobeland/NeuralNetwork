using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{

    [Serializable]
    public class Layer
    {

        public List<ActiveNeuron> NeuronsInLayer { get; set; }

        public Layer(int numberOfNeuronsInLayer, List<Neuron> connectionsIn)
        {
            NeuronsInLayer = new List<ActiveNeuron>();
            for (int i = 0; i < numberOfNeuronsInLayer; i++)
            {
                ActiveNeuron n = new ActiveNeuron(connectionsIn);
                n.initBias();
                NeuronsInLayer.add(n);
            }
        }

        public Layer(int numberOfNeuronsInLayer, List<ActiveNeuron> connectionsIn, int bias)
        {
            NeuronsInLayer = new List<ActiveNeuron>();
            for (int i = 0; i < numberOfNeuronsInLayer; i++)
            {
                ActiveNeuron n = new ActiveNeuron(connectionsIn, bias);
                n.initBias();
                NeuronsInLayer.add(n);
            }
        }

        public void fireAll()
        {
            foreach (ActiveNeuron n in NeuronsInLayer)
            {
                n.fire();
            }
        }


    }

}
