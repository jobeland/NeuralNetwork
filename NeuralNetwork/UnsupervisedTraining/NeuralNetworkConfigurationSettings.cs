using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class NeuralNetworkConfigurationSettings
    {
        public int NumInputNeurons { get; set; }
        public int NumHiddenNeurons { get; set; }
        public int NumHiddenLayers { get; set; }
        public int NumOutputNeurons { get; set; }
        public ISummationFunction SummationFunction { get; set; }
        public IActivationFunction ActivationFunction { get; set; }
    }
}
