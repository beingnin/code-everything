using System;
using System.Collections;
using System.Linq;

namespace Algorithms
{
    public static class Array
    {
        public static void Shuffle<T>(this T[] array, int seed)
        {
            //Fisher-Yates algorithm AKA the Knuth Shuffle
            var rnd = new Random(seed);
            int n = array.Length;
            while (n > 1)
            {
                int swappableIndex = rnd.Next(n--);
                T temp = array[n];
                array[n] = array[swappableIndex];
                array[swappableIndex] = temp;
            }
        }
        public static void Unshuffle<T>(this T[] array, int seed)
        {
            var rnd = new Random(seed);
            int n = array.Length;
            int[] rSequence = new int[n - 1];
            for (int i = 0; i < array.Length - 1; i++)
            {
                rSequence[i] = rnd.Next(n--);
            }
            for (int i = 1; i < array.Length; i++)
            {
                int swappableIndex = rSequence[^i];
                T temp = array[i];
                array[i] = array[swappableIndex];
                array[swappableIndex] = temp;
            }
        }
    }
}
