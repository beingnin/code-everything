using System;
using System.Collections.Generic;

namespace HRMS.Timesheet
{
    class Program
    {
        private static IDictionary<string, int> _relativeDiffs = new Dictionary<string, int>
        {
            { "YESTERDAY",-1},
            { "Y",-1},
            { "TODAY",0},
            { "T",0},
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Sprint Name?");
            var sprint = Console.ReadLine();
            Console.WriteLine("\nEnter date range in dd/MM/yyyy format. Type \"help\" & ENTER for showing format helpers\n");
            var userInput = Console.ReadLine();
            if (userInput.ToUpper() == "HELP")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nDate range formats can be given in the below formats");
                Console.WriteLine("\n````````````````````````````````````````````````````");
                Console.WriteLine("\n Single date\t\t\t:\t25/02/2020");
                Console.WriteLine("\n Multiple dates\t\t\t:\t25/02/2020,05/03/2020,09/04/2020");
                Console.WriteLine("\n Date range\t\t\t:\t12/01/2020-25/02/2020");
                Console.WriteLine("\n Multiple date ranges\t\t:\t12/01/2020-25/02/2020,03/03/2020-20/03/2020");
                Console.WriteLine("\n Mix of dates and date ranges\t:\t12/01/2020-25/02/2020,10/05/2020,13/05/2020,03/03/2020-20/03/2020");
                Console.WriteLine("\n Relative dates\t\t\t:\tyesterday(y), today(t)");

                Console.ResetColor();
                Console.WriteLine("\nEnter date range in dd/MM/yyyy format.");
                userInput = Console.ReadLine();
            }
            if (new List<string> { "YESTERDAY", "TODAY", "T", "Y" }.Contains(userInput.ToUpper()))
            {
                userInput = DateTime.Now.AddDays(_relativeDiffs[userInput.ToUpper()]).ToString("dd'/'MM'/'yyyy");
            }

            var ts = new Timesheet();
            var logs = ts.GetLogFromRange(userInput);
            ts.Log(sprint, logs);


            Console.ReadKey();
        }
    }
}
