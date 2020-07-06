using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.TimeSheet
{
    class Program
    {
        static void Main(string[] args)
        {
            string sprint = null, range = null;
            string userInput = null;
            Console.WriteLine("Do you want to start setup? y/n");
            userInput = Console.ReadKey().Key.ToString();
            if (userInput.ToUpper() == "Y")
            {
                Console.WriteLine("\nDo you want to setup sprint? y/n");
                if (Console.ReadKey().Key.ToString().ToUpper() == "Y")
                {
                    Console.WriteLine("\nEnter sprint name without number. example :- SP2020-EGT1 (case sensitive)");
                    sprint = Console.ReadLine();
                }
                Console.WriteLine("\nDo you want to log time for custom date range. If no, dates and time will be taken from your biometric inputs? y/n");
                if (Console.ReadKey().Key.ToString().ToUpper() == "Y")
                {
                    Console.WriteLine("\nEnter date range in dd/MM/yyyy format. Press help & ENTER for showing format helpers\n");
                    userInput = Console.ReadLine();
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

                        Console.ResetColor();
                        Console.WriteLine("\nEnter date range in dd/MM/yyyy format.");
                        userInput = Console.ReadLine();
                    }
                    range = userInput;
                }
            }

            //start logging time
            try
            {

                new IMS().BookMyLog(sprint, range);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            //exit
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
