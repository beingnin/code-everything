using System;
using System.Threading.Tasks;

namespace Canvas.Core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int seed = 0;

            Func<int> inc = delegate () { return seed++; };

            Console.WriteLine(inc());
            Console.WriteLine(inc());
            Console.WriteLine(seed);


        }
        static int Add(int a)
        {
            return a++;
        }

        static Task Wait()
        {
            return Task.Delay(1000);
        }
    }
}
