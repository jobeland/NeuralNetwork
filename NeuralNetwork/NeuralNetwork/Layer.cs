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
        private readonly IList<INeuron> _neuronsInLayer;

        public Layer(IList<INeuron> neuronsInLayer)
        {
            _neuronsInLayer = neuronsInLayer;
            //NeuronsInLayer = new List<ActiveNeuron>();
            //for (int i = 0; i < numberOfNeuronsInLayer; i++)
            //{
            //    ActiveNeuron n = new ActiveNeuron(connectionsIn, activationFunction);
            //    n.initBias();
            //    NeuronsInLayer.Add(n);
            //}
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
