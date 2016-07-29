using System;

namespace ArtificialNeuralNetwork.Genes
{
    public class SomaGene : IEquatable<SomaGene>
    {
        public double Bias;
        public Type SummationFunction;

        #region Equality Members
        /// <summary>
        /// Returns true if the fields of the SomaGene objects are the same.
        /// </summary>
        /// <param name="obj">The SomaGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the SomaGene objects are the same; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as SomaGene);
        }

        /// <summary>
        /// Returns true if the fields of the SomaGene objects are the same.
        /// </summary>
        /// <param name="somaGene">The SomaGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the SomaGene objects are the same; false otherwise.
        /// </returns>
        public bool Equals(SomaGene somaGene)
        {
            if (somaGene == null)
            {
                return false;
            }

            if (somaGene.Bias != Bias || somaGene.SummationFunction != SummationFunction)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the fields of the SomaGene objects are the same.
        /// </summary>
        /// <param name="a">The SomaGene object to compare.</param>
        /// <param name="b">The SomaGene object to compare.</param>
        /// <returns>
        /// True if the objects are the same, are both null, or have the same values;
        /// false otherwise.
        /// </returns>
        public static bool operator ==(SomaGene a, SomaGene b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            // If one or the other is null, return false.
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(SomaGene a, SomaGene b)
        {
            return !(a == b);
        }

        // Following this algorithm: http://stackoverflow.com/a/263416
        /// <summary>
        /// Returns the hash code of the SomaGene.
        /// </summary>
        /// <returns>The hash code of the SomaGene.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Bias.GetHashCode() ^ SummationFunction.GetHashCode();
                return hash;
            }
        }
        #endregion
    }
}
