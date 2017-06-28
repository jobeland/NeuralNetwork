using ArtificialNeuralNetwork.Genes;
using System;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Neuron : INeuron
    {
        public ISoma Soma { get; set; }
        public IAxon Axon { get; set; }

        public Neuron(ISoma soma, IAxon axon)
        {
            Soma = soma;
            Axon = axon;
        }

        public static INeuron GetInstance(ISoma soma, IAxon axon)
        {
            return new Neuron(soma, axon);
        }

        public void Process()
        {
            Axon.ProcessSignal(Soma.CalculateSummation());
        }

        public NeuronGene GetGenes()
        {
            return new NeuronGene
            {
                Soma = Soma.GetGenes(),
                Axon = Axon.GetGenes()
            };
        }
    }
}
