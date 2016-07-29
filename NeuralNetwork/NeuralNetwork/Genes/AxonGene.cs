using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.Genes
{
    public class AxonGene : IEquatable<AxonGene>
    {
        public Type ActivationFunction { get; set; }
        public IList<double> Weights { get; set; }

        #region Equality Members
        /// <summary>
        /// Returns true if the fields of the AxonGene objects are the same.
        /// </summary>
        /// <param name="obj">The AxonGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the AxonGene objects are the same; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as AxonGene);
        }

        /// <summary>
        /// Returns true if the fields of the AxonGene objects are the same.
        /// </summary>
        /// <param name="axonGene">The AxonGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the AxonGene objects are the same; false otherwise.
        /// </returns>
        public bool Equals(AxonGene axonGene)
        {
            if (axonGene == null)
            {
                return false;
            }

            if (axonGene.ActivationFunction != ActivationFunction || axonGene.Weights.Count != Weights.Count)
            {
                return false;
            }

            const double tolerance = 0.00001;
            return !Weights.Where((t, i) => Math.Abs(t - axonGene.Weights[i]) > tolerance).Any();
        }

        /// <summary>
        /// Returns true if the fields of the AxonGene objects are the same.
        /// </summary>
        /// <param name="a">The AxonGene object to compare.</param>
        /// <param name="b">The AxonGene object to compare.</param>
        /// <returns>
        /// True if the objects are the same, are both null, or have the same values;
        /// false otherwise.
        /// </returns>
        public static bool operator ==(AxonGene a, AxonGene b)
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

        public static bool operator !=(AxonGene a, AxonGene b)
        {
            return !(a == b);
        }

        // Following this algorithm: http://stackoverflow.com/a/263416
        /// <summary>
        /// Returns the hash code of the AxonGene.
        /// </summary>
        /// <returns>The hash code of the AxonGene.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Weights.GetHashCode();
                return hash;
            }
        }
        #endregion
    }
}
