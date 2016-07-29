using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.Genes
{
    public class NeuralNetworkGene : IEquatable<NeuralNetworkGene>
    {
        public LayerGene InputGene;
        public IList<LayerGene> HiddenGenes;
        public LayerGene OutputGene;

        #region Equality Members
        /// <summary>
        /// Returns true if the fields of the NeuralNetworkGene objects are the same.
        /// </summary>
        /// <param name="obj">The NeuralNetworkGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the NeuralNetworkGene objects are the same; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as NeuralNetworkGene);
        }

        /// <summary>
        /// Returns true if the fields of the NeuralNetworkGene objects are the same.
        /// </summary>
        /// <param name="neuralNetworkGene">The NeuralNetworkGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the NeuralNetworkGene objects are the same; false otherwise.
        /// </returns>
        public bool Equals(NeuralNetworkGene neuralNetworkGene)
        {
            if (neuralNetworkGene == null)
            {
                return false;
            }

            if (neuralNetworkGene.InputGene != InputGene || neuralNetworkGene.OutputGene != OutputGene ||
                neuralNetworkGene.HiddenGenes.Count != HiddenGenes.Count)
            {
                return false;
            }

            return !HiddenGenes.Where((t, i) => t != neuralNetworkGene.HiddenGenes[i]).Any();
        }

        /// <summary>
        /// Returns true if the fields of the NeuralNetworkGene objects are the same.
        /// </summary>
        /// <param name="a">The NeuralNetworkGene object to compare.</param>
        /// <param name="b">The NeuralNetworkGene object to compare.</param>
        /// <returns>
        /// True if the objects are the same, are both null, or have the same values;
        /// false otherwise.
        /// </returns>
        public static bool operator ==(NeuralNetworkGene a, NeuralNetworkGene b)
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

        public static bool operator !=(NeuralNetworkGene a, NeuralNetworkGene b)
        {
            return !(a == b);
        }

        // Following this algorithm: http://stackoverflow.com/a/263416
        /// <summary>
        /// Returns the hash code of the NeuralNetworkGene.
        /// </summary>
        /// <returns>The hash code of the NeuralNetworkGene.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ InputGene.GetHashCode() ^ HiddenGenes.GetHashCode() ^ OutputGene.GetHashCode();
                return hash;
            }
        }
        #endregion
    }
}
