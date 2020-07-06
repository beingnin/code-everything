using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.WorkFromHome
{
    class Program
    {
        static void Main(string[] args)
        {

            string range = null;
            string userInput = null;




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
            try
            {

                new IMS().RequestWFH(range);
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
