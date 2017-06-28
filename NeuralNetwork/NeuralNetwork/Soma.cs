using ArtificialNeuralNetwork.Genes;
using System.Collections.Generic;

namespace ArtificialNeuralNetwork
{
    public class Soma : ISoma
    {
        public IList<Synapse> Dendrites { get; set; }
        public ISummationFunction SummationFunction { get; set; }
        public double Bias { get; set; }
        public double Value { get; private set; }

        public Soma(IList<Synapse> dendrites, ISummationFunction summationFunction, double bias)
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
            return Value = SummationFunction.CalculateSummation(Dendrites, Bias);
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
