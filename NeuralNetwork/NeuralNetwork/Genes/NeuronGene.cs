using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialNeuralNetwork.Genes
{
    public class NeuronGene : IEquatable<NeuronGene>
    {
        public SomaGene Soma;
        public AxonGene Axon;

        #region Equality Members
        /// <summary>
        /// Returns true if the fields of the NeuronGene objects are the same.
        /// </summary>
        /// <param name="obj">The NeuronGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the NeuronGene objects are the same; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as NeuronGene);
        }

        /// <summary>
        /// Returns true if the fields of the NeuronGene objects are the same.
        /// </summary>
        /// <param name="NeuronGene">The NeuronGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the NeuronGene objects are the same; false otherwise.
        /// </returns>
        public bool Equals(NeuronGene neuronGene)
        {
            if (neuronGene == null)
            {
                return false;
            }

            if (neuronGene.Axon != Axon || neuronGene.Soma != Soma)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the fields of the NeuronGene objects are the same.
        /// </summary>
        /// <param name="a">The NeuronGene object to compare.</param>
        /// <param name="b">The NeuronGene object to compare.</param>
        /// <returns>
        /// True if the objects are the same, are both null, or have the same values;
        /// false otherwise.
        /// </returns>
        public static bool operator ==(NeuronGene a, NeuronGene b)
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

        public static bool operator !=(NeuronGene a, NeuronGene b)
        {
            return !(a == b);
        }

        // Following this algorithm: http://stackoverflow.com/a/263416
        /// <summary>
        /// Returns the hash code of the NeuronGene.
        /// </summary>
        /// <returns>The hash code of the NeuronGene.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Axon.GetHashCode() ^ Soma.GetHashCode();
                return hash;
            }
        }
        #endregion
    }
}
