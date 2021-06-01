using System;
using System.Reflection;

namespace Botsay.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var versionString = Assembly.GetEntryAssembly()
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                        .InformationalVersion
                                        .ToString();

                Console.WriteLine($"botsay v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("  botsay <message>");
                return;
            }
        }
    }
}
