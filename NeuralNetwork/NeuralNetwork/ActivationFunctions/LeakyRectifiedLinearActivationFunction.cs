namespace ArtificialNeuralNetwork.ActivationFunctions
{
    public class LeakyRectifiedLinearActivationFunction : IActivationFunction
    {
        public double CalculateActivation(double signal)
        {
            return signal > 0.0 ? signal : 0.01*signal;
        }
    }
}
