using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace AuctionServiceWebApi
{
    public class Program
    {

        static void Main()
        {
            var url = ConfigurationManager.AppSettings["apiUrl"];
            try
            {
                using (WebApp.Start<Startup>(url: url))
                {
                    Console.WriteLine("Owin-Server is up & running!");
                    Console.WriteLine("Press any key to stop the server");
                    Console.ReadKey();
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Exception has occurred: {exc.Message}");
                Console.ReadKey();
            }
        }
    }
}