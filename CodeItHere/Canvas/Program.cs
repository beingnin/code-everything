using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas
{
    class Program
    {
        static unsafe void Main(string[] args)
        {


            Pointers();
            Console.ReadKey();
        }
        static unsafe void Pointers()
        {
            DateTime[] arr = new DateTime[3]
            {
                new DateTime(2020, 01, 01),
                new DateTime(2020, 01, 02),
                new DateTime(2020, 01, 03)
            };
            fixed (DateTime* p = &arr[0])
            {
                DateTime* p2 = p;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                p2++;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                p2++;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                *p = (*p).AddDays(10);
                Console.WriteLine($"memory address : { (int)p} and value is {*p}");
            }


        }
    }
}
