using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork;
using BasicGame;

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
            Game g = new Game(10, 10);
            while(!g.IsGameWon() && !g.IsGameLost())
            {
                nn.setInputs(new []{1.0}); //TODO: map turn options to input
                nn.calculate();
                nn.GetOutput(); //TODO: map output to action
            }
            double result = 0;
            if (g.IsGameWon())
            {
                result = g.MovesLeft;
            }
            else if (g.IsGameLost())
            {
                result -= g.GetDotsLeft();
            }
			double[] eval = {result};//Add in evaluation of scenario
			average += eval[0];
		}
		average = average / (double) numberOfScenariosToRun;
		alg.AddEval(index, average);
	}

}
}
