using ArtificialNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining.Utils
{
    public class NeuralNetworkSaver : INeuralNetworkSaver
    {
        private readonly string _directory;

        public NeuralNetworkSaver(string directory)
        {
            _directory = directory;
        }

        public string SaveNeuralNetwork(INeuralNetwork network, double networkEvaluation, int epoch)
        {
            var genes = network.GetGenes();
            var json = JsonConvert.SerializeObject(genes);
            var filename = string.Format("\\network_eval_{0}_epoch_{1}_date_{2}.json", networkEvaluation, epoch, DateTime.Now.Ticks);
            File.WriteAllText(_directory + filename, json);
            return filename;
        }
    }
}
