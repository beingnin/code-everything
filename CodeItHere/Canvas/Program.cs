using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Canvas
{


    class Program
    {
        static void Main(string[] args)
        {
            int num = 15017;
            Console.WriteLine(EntryPoint(num));
            Console.ReadKey();

        }
        static bool EntryPoint(int num)
        {
            return CheckDivisibilty(num, num/2);
        }
        static bool CheckDivisibilty(int num,int checkTill)
        {
            int checkingNum = 2;
            if (checkingNum> checkTill) return true;
            if (num % checkingNum == 0)
                return false;
            return CheckDivisibilty(num, ++checkingNum);
        }

        public static void RegisterMimeTypes(IDictionary<string, string> mimeTypes)
        {
            if (mimeTypes == null || mimeTypes.Count == 0)
                return;
            var field = typeof(System.Web.MimeMapping).GetField("_mappingDictionary",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Static);

            var currentValues = field.GetValue(null);
            var add = field.FieldType.GetMethod("AddMapping",
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance);

            foreach (var mime in mimeTypes)
            {
                add.Invoke(currentValues, new object[] { mime.Key, mime.Value });
            }

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
