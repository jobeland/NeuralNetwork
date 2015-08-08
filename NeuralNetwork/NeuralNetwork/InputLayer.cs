//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ArtificialNeuralNetwork
//{

//    [Serializable]
//    public class InputLayer
//    {

//        public List<Neuron> NeuronsInLayer { get; set; }

//        public InputLayer(int numberOfNeuronsInLayer)
//        {
//            NeuronsInLayer = new List<Neuron>();
//            for (int i = 0; i < numberOfNeuronsInLayer; i++)
//            {
//                Neuron n = new Neuron();
//                NeuronsInLayer.Add(n);
//            }
//        }
//        public void SetInputs(double[] inputs)
//        {
//            int length = inputs.Length;
//            if (length != NeuronsInLayer.Count)
//            {
//                Console.WriteLine("Warning: length of inputs does not match input layer");
//                if (length > NeuronsInLayer.Count)
//                {
//                    length = NeuronsInLayer.Count;
//                }
//            }
//            for (int i = 0; i < length; i++)
//            {
//                NeuronsInLayer[i].Input = inputs[i];
//            }
//        }

//        public void FireAll()
//        {
//            foreach (Neuron n in NeuronsInLayer)
//            {
//                n.Fire();
//            }
//        }
//    }

//}
