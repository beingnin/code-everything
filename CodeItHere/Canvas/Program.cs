using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/* Don't change anything here.
 * Do not add any other imports. You need to write
 * this program using only these libraries 
 */

namespace ProgramNamespace
{
    /* You may add helper classes here if necessary */

    public class Program
    {
        public static List<String> processData(
                                        IEnumerable<string> lines)
        {
            /* 
             * Do not make any changes outside this method.
             *
             * Modify this method to process `array` as indicated
             * in the question. At the end, return a List containing
             * the appropriate values
             *
             * Do not print anything in this method
             *
             * Submit this entire program (not just this function)
             * as your answer
             */

            //the first item is customer name,
            //the second item is the data - center location,
            //the third item is the API name,
            //the fourth item is the API level being used by that customer from that data center in string.
            //the fifth item is the API level being used by that customer from that data center(extracted number).


            var subscriptions = lines.Select(x =>
            {
                var splits = x.Split(',');
                return Tuple.Create(splits[0], splits[1], splits[2], splits[3], Convert.ToInt32(new Regex(@"\d+").Match(splits[3]).Value));
            });


            var levels = subscriptions.GroupBy(x => x.Item3)
                                      .Select(x => Tuple.Create(x.Key, x.Select(y => Convert.ToInt32(new Regex(@"\d+").Match(y.Item4).Value)).Max()));

            var deprecatedList= subscriptions.Select(subscription => subscription.Item5 < levels.First(x => x.Item1 == subscription.Item3).Item2 ? subscription :null)
                                 .Where(x => x !=null);

            return subscriptions.Where(x => deprecatedList.Where(y=>y.Item1 == x.Item1).Count()==0).Select(x=>x.Item1).ToList();


        }

        static void Main(string[] args)
        {
            try
            {
                String line;
                var inputLines = new List<String>();
                while ((line = Console.ReadLine()) != "q")
                {
                    line = line.Trim();
                    if (line != "")
                        inputLines.Add(line);
                }
                var retVal = processData(inputLines);
                foreach (var res in retVal)
                    Console.WriteLine(res);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

    }
}
