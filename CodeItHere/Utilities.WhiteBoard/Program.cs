using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Utilities.WhiteBoard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //new WebHostBuilder().UseKestrel().UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build().Run();
            CreateHostBuilder(args).Build().Run();
            //Console.WriteLine("server is "+System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
