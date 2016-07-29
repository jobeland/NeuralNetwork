using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.Genes
{
    public class LayerGene : IEquatable<LayerGene>
    {
        public IList<NeuronGene> Neurons;

        #region Equality Members
        /// <summary>
        /// Returns true if the fields of the LayerGene objects are the same.
        /// </summary>
        /// <param name="obj">The LayerGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the LayerGene objects are the same; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as LayerGene);
        }

        /// <summary>
        /// Returns true if the fields of the LayerGene objects are the same.
        /// </summary>
        /// <param name="layerGene">The LayerGene object to compare with.</param>
        /// <returns>
        /// True if the fields of the LayerGene objects are the same; false otherwise.
        /// </returns>
        public bool Equals(LayerGene layerGene)
        {
            if (layerGene == null)
            {
                return false;
            }

            if (layerGene.Neurons.Count != Neurons.Count)
            {
                return false;
            }

            return !Neurons.Where((t, i) => t != layerGene.Neurons[i]).Any();
        }

        /// <summary>
        /// Returns true if the fields of the LayerGene objects are the same.
        /// </summary>
        /// <param name="a">The LayerGene object to compare.</param>
        /// <param name="b">The LayerGene object to compare.</param>
        /// <returns>
        /// True if the objects are the same, are both null, or have the same values;
        /// false otherwise.
        /// </returns>
        public static bool operator ==(LayerGene a, LayerGene b)
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

        public static bool operator !=(LayerGene a, LayerGene b)
        {
            return !(a == b);
        }

        // Following this algorithm: http://stackoverflow.com/a/263416
        /// <summary>
        /// Returns the hash code of the LayerGene.
        /// </summary>
        /// <returns>The hash code of the LayerGene.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Neurons.GetHashCode();
                return hash;
            }
        }
        #endregion
    }
}
