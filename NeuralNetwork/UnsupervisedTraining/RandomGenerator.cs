using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedTraining
{
    public class RandomGenerator
    {
        private static readonly object padlock = new object();

        private static Random random;

        public static Random GetInstance()
        {
            lock (padlock)
            {
                if (random == null)
                {
                    random = new Random();
                }
            }
            return random; ;
        }

    }
}
