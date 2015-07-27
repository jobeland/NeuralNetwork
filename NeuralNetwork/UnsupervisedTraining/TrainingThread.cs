using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork;
using BasicGame;

namespace UnsupervisedTraining
{

    public class TrainingThread
    {

        private int index;
        private NeuralNetwork nn;
        private GeneticAlgorithm alg;

        public TrainingThread(NeuralNetwork nn, int index, GeneticAlgorithm alg)
        {
            this.index = index;
            this.nn = nn;
            this.alg = alg;
        }

        public void ThreadRun()
        {
            double average = 0;
            int numberOfScenariosToRun = 1;
            Array values = Enum.GetValues(typeof(MoveDirection));
            for (int i = 0; i < numberOfScenariosToRun; i++)
            {
                Game g = new Game(10, 10, 300);
                while (!g.IsGameWon() && !g.IsGameLost())
                {
                    MoveDirection dirToMove = MoveDirection.DOWN;
                    double highestProb = double.MinValue;
                    foreach (MoveDirection val in values)
                    {
                        double distance = g.GetDistanceToClosestDot(val, g.CurrentCoord, new List<Tuple<int, int>>());
                        nn.setInputs(new[] { distance });
                        nn.calculate();
                        double probability = nn.GetOutput();
                        if (probability > highestProb)
                        {
                            dirToMove = val;
                            highestProb = probability;
                        }
                    }
                    g.UseTurn(dirToMove);
                }
                double result = 0;
                result = g.MovesLeft + (g.Width * g.Length) - g.GetDotsLeft();
                //if (g.IsGameWon())
                //{
                //    result = g.MovesLeft + (g.Width * g.Length) - g.GetDotsLeft();
                //}
                //else if (g.IsGameLost())
                //{
                //    result = g.GetDotsLeft();
                //}
                double[] eval = { result };
                average += eval[0];
            }
            average = average / (double)numberOfScenariosToRun;
            alg.AddEval(index, average);
        }



    }
}
