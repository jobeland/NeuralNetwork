namespace ArtificialNeuralNetwork.ActivationFunctions
{
    public class RectifiedLinearActivationFunction : IActivationFunction
    {
        public double CalculateActivation(double signal)
        {
            return signal > 0 ? signal : 0.0;
        }
    }
}
