using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class Program
    {
        static void Main(string[] args)
        {
            ArrayShufflingExample();
        }

        static void ArrayShufflingExample()
        {
            Console.WriteLine("Original");
            var random = new Random();
            var array = new int[] { 1, 5, 6, 8, 23, 56, 98, 456, 255, 266, 666, 666, 2, 4, 9 };
            //var array = Enumerable.Range(1, 10).Select(x=>random.Next(100)).ToArray();
            Console.WriteLine(string.Join(',', array.Select(x => x.ToString())));

            Console.WriteLine("Shuffled");
            array.Shuffle<int>(25);
            Console.WriteLine(string.Join(',', array.Select(x => x.ToString())));

            Console.WriteLine("Reshuffled");
            array.Unshuffle<int>(25);
            Console.WriteLine(string.Join(',', array.Select(x => x.ToString())));

        }
    }
}
