using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Neuron : INeuron
    {
        private readonly ISoma _soma;
        private readonly IAxon _axon;

        public Neuron(ISoma soma, IAxon axon)
        {
            _soma = soma;
            _axon = axon;
        }

        public virtual double CalculateActivationFunction()
        {
            return 0.0;
        }

        public void ProcessInput()
        {
            _axon.ProcessSignal(_soma.CalculateSummation());
        }
	
    //public void initBias(){
    //    double val = new Random().NextDouble();
    //    if(new Random().NextDouble() < 0.5){
    //        // 50% chance of being negative, being between -1 and 1
    //        val = 0 - val;
    //    }
    //    Bias = val;
    //}
	
    //private void initializeWeights(){
    //    // weights assumed to always be between -1 and 1
    //    for(int i = 0; i < Weights.Length; i++){
    //        double val = new Random().NextDouble();
    //        if(new Random().NextDouble() < 0.5){
    //            // 50% chance of being negative, being between -1 and 1
    //            val = 0 - val;
    //        }
    //        Weights[i] = val;
    //    }
    //}
	
    }
}
