using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        static void Main(string[] args)
        {
            try
            {
                Bira();
                Console.ReadKey();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public static void Bira()
        {
            //inputs
            Console.WriteLine("Enter number of Bira cartons, number of people in the grp(separated by space)");
            var split = Console.ReadLine().Split(' ');
            int NoBiraCartons = Convert.ToInt32(split[0]);
            int NoPeople = Convert.ToInt32(split[1]);
            Console.WriteLine("Enter the weight of each bira carton(separated by space & should be more than 0)");
            split = Console.ReadLine().Split(' ');
            var cartonWeights = new int[NoBiraCartons];
            for (int i = 0; i < NoBiraCartons; i++)
            {
                cartonWeights[i] = Convert.ToInt32(split[i]);
            }
            Console.WriteLine("Enter the weight of each person (separated by space & should be more than 0)");
            split = Console.ReadLine().Split(' ');
            var peopleWeight = new int[NoPeople];
            for (int i = 0; i < NoPeople; i++)
            {
                peopleWeight[i] = Convert.ToInt32(split[i]);
            }

            //sorting

            Array.Sort(cartonWeights);
            Array.Sort(peopleWeight);
            int pendingCartons = NoBiraCartons;
            int minutesTaken = 0;

            if (cartonWeights.Last() > peopleWeight.Last())//no one can lift it
                return;

            while (pendingCartons > 0)
            {
                var lot = 0;
                //assign people with cartons according to their weights
                for (int i = 0; i < NoPeople; i++)
                {
                    for (int j = 0; j < NoBiraCartons; j++)
                    {
                        if (peopleWeight[i] >= cartonWeights[j] && cartonWeights[j]!=0)
                        {
                            lot++;
                            cartonWeights[j] = 0;
                            break;
                        }
                    }                    
                }
                minutesTaken++;
                pendingCartons = pendingCartons - lot;
                if (pendingCartons > 0)
                    minutesTaken++;// return time in case more cartons left
            }

            Console.WriteLine(minutesTaken);

            Console.ReadKey();
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


    struct Version : IEquatable<Version>, IComparable<Version>
    {

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        private Version(int Major, int Minor, int Patch)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Patch = Patch;
        }
        public static implicit operator Version(string v)
        {
            var items = v.Split('.');
            if (items.Length != 3)
                throw new InvalidCastException("The provided version must include exactly three parts(major, minor & patch)");
            if (!int.TryParse(items[0], out int mj))
                throw new InvalidCastException("All parts of a version should be a whole number");
            if (!int.TryParse(items[1], out int mn))
                throw new InvalidCastException("All parts of a version should be a whole number");
            if (!int.TryParse(items[2], out int p))
                throw new InvalidCastException("All parts of a version should be a whole number");
            return new Version(mj, mn, p);
        }
        public static bool operator >(Version lhs, Version rhs)
        {
            //query:  lhs > rhs
            //example : 1.2.3 > 1.0.3

            if (lhs.Major > rhs.Major)
                return true;
            if (lhs.Major < rhs.Major)
                return false;
            if (lhs.Minor > rhs.Minor)
                return true;
            if (lhs.Minor < rhs.Minor)
                return false;
            if (lhs.Patch > rhs.Patch)
                return true;
            if (lhs.Patch < rhs.Patch)
                return false;
            return false;
        }
        public static bool operator <(Version lhs, Version rhs)
        {
            return rhs > lhs;
        }
        public static bool operator >=(Version lhs, Version rhs)
        {
            return lhs > rhs || lhs == rhs;
        }
        public static bool operator <=(Version lhs, Version rhs)
        {
            return lhs < rhs || lhs == rhs;
        }
        public static bool operator ==(Version lhs, Version rhs)
        {
            if ((lhs.Major == rhs.Major) && (lhs.Minor == rhs.Minor) && (lhs.Patch == rhs.Patch))
                return true;
            return false;
        }
        public static bool operator !=(Version lhs, Version rhs)
        {
            return !(lhs == rhs);
        }
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public bool Equals(Version other)
        {
            return this == other;
        }

        public int CompareTo(Version other)
        {
            if (this > other)
                return 1;
            if (this < other)
                return -1;
            return 0;
        }
        public override bool Equals(object obj)
        {
            return this == (Version)obj;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.Major.GetHashCode();
                hash = hash * 23 + this.Minor.GetHashCode();
                hash = hash * 23 + this.Patch.GetHashCode();
                return hash;
            }
        }

    }


}
