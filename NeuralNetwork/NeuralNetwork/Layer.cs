﻿using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{

    [Serializable]
    public class Layer
    {

        public List<ActiveNeuron> NeuronsInLayer { get; set; }

        public Layer(int numberOfNeuronsInLayer, List<Neuron> connectionsIn, IActivationFunction activationFunction)
        {
            NeuronsInLayer = new List<ActiveNeuron>();
            for (int i = 0; i < numberOfNeuronsInLayer; i++)
            {
                ActiveNeuron n = new ActiveNeuron(connectionsIn, activationFunction);
                n.initBias();
                NeuronsInLayer.Add(n);
            }
        }

        public Layer(int numberOfNeuronsInLayer, List<ActiveNeuron> connectionsIn, int bias, IActivationFunction activationFunction)
        {
            NeuronsInLayer = new List<ActiveNeuron>();
            for (int i = 0; i < numberOfNeuronsInLayer; i++)
            {
                ActiveNeuron n = new ActiveNeuron(connectionsIn, bias, activationFunction);
                n.initBias();
                NeuronsInLayer.Add(n);
            }
        }

        public void FireAll()
        {
            foreach (ActiveNeuron n in NeuronsInLayer)
            {
                n.Fire();
            }
        }


    }

}
