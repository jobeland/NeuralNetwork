using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    
[Serializable]
public class ActiveNeuron : Neuron{
	
	public Double[] weights {get; set;}  
	public List<Neuron> connectionsIn {get; set;}
	public double bias  {get; set;}
	

	
	public ActiveNeuron(List<Neuron> neuronsIn) {
		connectionsIn = neuronsIn;
		weights = new Double[connectionsIn.Count];
		initializeWeights();
		bias = 0;
	}
	
	public ActiveNeuron(List<ActiveNeuron> neuronsIn, int bias) {
		connectionsIn = new List<Neuron>();
		connectionsIn.addAll(neuronsIn);
		weights = new Double[connectionsIn.Count];
		initializeWeights();
		bias = 0;
	}
	
	public void initBias(){
		double val = Math.random();
		if(Math.random() < 0.5){
			// 50% chance of being negative, being between -1 and 1
			val = 0 - val;
		}
		bias = val;
	}
	
	private void initializeWeights(){
		// weights assumed to always be between -1 and 1
		for(int i = 0; i < weights.length; i++){
			double val = Math.random();
			if(Math.random() < 0.5){
				// 50% chance of being negative, being between -1 and 1
				val = 0 - val;
			}
			weights[i] = val;
		}
	}

	private double sumInputsAndWeightsWithBias(){
		double sum = 0;
		for(int i = 0; i < weights.length; i++){
			sum += weights[i] * connectionsIn[i].output;
		}
		sum += this.bias;
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
		this.output = calculateThresholdActivationFunction();
		this.input = 0;
	}
	



	
	
	
	

}

}
