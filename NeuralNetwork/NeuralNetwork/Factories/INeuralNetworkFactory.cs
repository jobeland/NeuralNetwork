using ArtificialNeuralNetwork.Genes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Factories
{
    public interface INeuralNetworkFactory
    {
        INeuralNetwork Create(int numInputs, int numOutputs, int numHiddenLayers, int numHiddenPerLayer);
        INeuralNetwork Create(int numInputs, int numOutputs, IList<int> hiddenLayerSpecs);
        INeuralNetwork Create(NeuralNetworkGene genes);
    }
}
