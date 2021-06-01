using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Canvas
{
    class Club : IEnumerable<Person>
    {
        public List<Person> Players { get; set; }

        public IEnumerator<Person> GetEnumerator()
        {
            return this.Players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Players.GetEnumerator();
        }
    }
    public class Person
    {
        public Person(string name, int age)
        {
            this.Name = name;
            Age = age;
        }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {

        static async Task Main(string[] args)
        {
            try
            {
                var p = new Person("p:" + DateTime.Now.Ticks, 25);
                await new MongoTransactionalRepo().RunTransaction(p);
                Console.ReadKey();
            }
            catch (Exception ex)
            {

                throw ex;
            }


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
