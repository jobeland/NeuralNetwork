using ArtificialNeuralNetwork.Genes;
using ArtificialNeuralNetwork.SummationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    public class Soma : ISoma
    {
        private readonly IList<Synapse> _dendrites;
        private readonly ISummationFunction _summationFunction;
        private readonly double _bias;

        private Soma(IList<Synapse> dendrites, ISummationFunction summationFunction, double bias)
        {
            _dendrites = dendrites;
            _summationFunction = summationFunction;
            _bias = bias;
        }

        public static ISoma GetInstance(IList<Synapse> dendrites, ISummationFunction summationFunction, double bias)
        {
            return new Soma(dendrites, summationFunction, bias);
        }

        public double CalculateSummation()
        {
            return _summationFunction.CalculateSummation(_dendrites, _bias);
        }

        public SomaGene GetGenes()
        {
            var gene = new SomaGene
            {
                Bias = _bias,
            };
            gene.Weights = _dendrites.Select(d => d.Weight).ToList();
            gene.SummationFunction = determineSupportedSummationFunction();
            return gene;
        }

        internal SupportedSummationFunctions determineSupportedSummationFunction()
        {
            var type = _summationFunction.GetType();

            if (type == typeof(AverageSummation))
            {
                return SupportedSummationFunctions.Average;
            }
            else if (type == typeof(MaxSummation))
            {
                return SupportedSummationFunctions.Max;
            }
            else if (type == typeof(MinSummation))
            {
                return SupportedSummationFunctions.Min;
            }
            else if (type == typeof(SimpleSummation))
            {
                return SupportedSummationFunctions.Simple;
            }
            else
            {
                throw new NotSupportedException(string.Format("{0} is not a supported summation function", type));
            }
        }


    }
}
