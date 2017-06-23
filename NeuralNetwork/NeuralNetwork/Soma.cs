using ArtificialNeuralNetwork.Genes;
using System.Collections.Generic;

namespace ArtificialNeuralNetwork
{
    public class Soma : ISoma
    {
        public IList<Synapse> Dendrites;
        public ISummationFunction SummationFunction;
        public double Bias;

        private Soma(IList<Synapse> dendrites, ISummationFunction summationFunction, double bias)
        {
            Dendrites = dendrites;
            SummationFunction = summationFunction;
            Bias = bias;
        }

        public static ISoma GetInstance(IList<Synapse> dendrites, ISummationFunction summationFunction, double bias)
        {
            return new Soma(dendrites, summationFunction, bias);
        }

        public double CalculateSummation()
        {
            return SummationFunction.CalculateSummation(Dendrites, Bias);
        }

        public SomaGene GetGenes()
        {
            return new SomaGene
            {
                Bias = Bias,
                SummationFunction = SummationFunction.GetType()
            };
        }
    }
}
