using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork;

namespace UnsupervisedTraining
{
    
public class TrainingThread{

	private int index;
	private NeuralNetwork nn;
	private GeneticAlgorithm alg;

	public TrainingThread(NeuralNetwork nn, int index, GeneticAlgorithm alg) {
		this.index = index;
		this.nn = nn;
		this.alg = alg;
	}

	public void ThreadRun() {
		double average = 0;
		int numberOfScenariosToRun = 1;
		for (int i = 0; i < numberOfScenariosToRun; i++) {
            //TODO: add in scenario here
			double[] eval = {0};//Add in evaluation of scenario
			average += eval[0];
		}
		average = average / (double) numberOfScenariosToRun;
		alg.AddEval(index, average);
	}

}
}
