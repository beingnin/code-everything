using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Canvas
{


    class Program
    {
        static unsafe void Main(string[] args)
        {
            IMain<object> main = new Main<Sub1>();
            IEnumerable<object> a = new List<Sub1>();


            Console.ReadKey();
            //Pointers();
            //FlaggedFeatureEnums();
            //FlaggedPermissionEnums();
            //var mimeTypes = new Dictionary<string, string>()
            //{
            //    { ".heic", "image/heic"},
            //    {".extn", "custom/mime" }
            //};

            //RegisterMimeTypes(mimeTypes);
            //Console.WriteLine(MimeMapping.GetMimeMapping("filename.HEIC"));
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
    public interface IMain<out T>
    {

    }
    public class Main<T> : IMain<T>
    {
        public T Detail { get; set; }
    }
    public class Sub1
    {

    }
    public class Sub2
    {

    }
}
