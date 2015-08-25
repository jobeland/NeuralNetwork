using ArtificialNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class NeuralNetworkLoader : INeuralNetworkLoader
    {
        private readonly string _directory;

         public NeuralNetworkLoader(string directory)
        {
            _directory = directory;
        }

        public INeuralNetwork LoadNeuralNetwork(string filename)
        {
            var jsonNet = File.ReadAllText(_directory + filename);
            INeuralNetwork network = JsonConvert.DeserializeObject<INeuralNetwork>(jsonNet);
            return network;
        }
    }
}
