using ArtificialNeuralNetwork.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork
{
    
[Serializable]
public class ActiveNeuron : Neuron{
	
	public Double[] Weights {get; set;}  
	public List<Neuron> ConnectionsIn {get; set;}
	public double Bias  {get; set;}
    private readonly IActivationFunction _activationFunction;
	

	
	public ActiveNeuron(List<Neuron> neuronsIn, IActivationFunction activationFunction) {
		ConnectionsIn = neuronsIn;
		Weights = new Double[ConnectionsIn.Count];
		initializeWeights();
		Bias = 0;
        _activationFunction = activationFunction;
	}
	
	public ActiveNeuron(List<ActiveNeuron> neuronsIn, int bias, IActivationFunction activationFunction) {
		ConnectionsIn = new List<Neuron>();
        ConnectionsIn.AddRange(neuronsIn);
		Weights = new Double[ConnectionsIn.Count];
		initializeWeights();
		bias = 0;
        _activationFunction = activationFunction;
	}
	
	public void initBias(){
        double val = new Random().NextDouble();
		if(new Random().NextDouble() < 0.5){
			// 50% chance of being negative, being between -1 and 1
			val = 0 - val;
		}
		Bias = val;
	}
	
	private void initializeWeights(){
		// weights assumed to always be between -1 and 1
		for(int i = 0; i < Weights.Length; i++){
			double val = new Random().NextDouble();
			if(new Random().NextDouble() < 0.5){
				// 50% chance of being negative, being between -1 and 1
				val = 0 - val;
			}
			Weights[i] = val;
		}
	}

	private double sumInputsAndWeightsWithBias(){
		double sum = 0;
		for(int i = 0; i < Weights.Length; i++){
			sum += Weights[i] * ConnectionsIn[i].Output;
		}
		sum += this.Bias;
		return sum;
	}
	
	private double calculateThresholdActivationFunction(){
		double resultOfSummation = sumInputsAndWeightsWithBias();
//		if(resultOfSummation >= 0){
//			return 1.0;
//		}else{
//			return 0.0;
//		}
		return _activationFunction.Calculate(resultOfSummation);
	}
	
	public override double CalculateActivationFunction(){
			return calculateThresholdActivationFunction();
	}
	
	public override void Fire() {
		this.Output = calculateThresholdActivationFunction();
		this.Input = 0;
	}
	



	
	
	
	

}

}
