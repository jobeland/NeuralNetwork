using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    
[Serializable]
public class ActiveNeuron : Neuron{
	
	public Double[] Weights {get; set;}  
	public List<Neuron> ConnectionsIn {get; set;}
	public double Bias  {get; set;}
	

	
	public ActiveNeuron(List<Neuron> neuronsIn) {
		ConnectionsIn = neuronsIn;
		Weights = new Double[ConnectionsIn.Count];
		initializeWeights();
		Bias = 0;
	}
	
	public ActiveNeuron(List<ActiveNeuron> neuronsIn, int bias) {
		ConnectionsIn = new List<Neuron>();
		ConnectionsIn.addAll(neuronsIn);
		Weights = new Double[ConnectionsIn.Count];
		initializeWeights();
		bias = 0;
	}
	
	public void initBias(){
		double val = Math.random();
		if(Math.random() < 0.5){
			// 50% chance of being negative, being between -1 and 1
			val = 0 - val;
		}
		Bias = val;
	}
	
	private void initializeWeights(){
		// weights assumed to always be between -1 and 1
		for(int i = 0; i < Weights.length; i++){
			double val = Math.random();
			if(Math.random() < 0.5){
				// 50% chance of being negative, being between -1 and 1
				val = 0 - val;
			}
			Weights[i] = val;
		}
	}

	private double sumInputsAndWeightsWithBias(){
		double sum = 0;
		for(int i = 0; i < Weights.length; i++){
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
		return Math.tanh(resultOfSummation);
	}
	
	protected override double calculateActivationFunction(){
			return calculateThresholdActivationFunction();
	}
	
	protected override void fire() {
		this.Output = calculateThresholdActivationFunction();
		this.Input = 0;
	}
	



	
	
	
	

}

}
