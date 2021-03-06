using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using EducomOpdrachtTaskScheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EducomOpdrachtTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString() + " | Console application started.");
            UpdateDatabase();
            Console.WriteLine(DateTime.Now.ToString() + " | Finished, console will close in 5 seconds...");
            Thread.Sleep(5000);
        }

        public static void UpdateDatabase()
        {
            if (DoPost(0))
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Weatherstation operation succesful.");
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Weatherstation operation aborted.");
            }

            if (DoPost(1))
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Weather forecast operation succesful.");
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Weather forecast operation aborted.");
            }
        }

        public static bool DoPost(int type)
        {
            try
            {
                string url = string.Empty;

                // Bepaal URL uit gegeven type
                if (type == 0)
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["weerstationsUrl"];
                }
                else
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["weerberichtenUrl"];
                }

                // Creëer een web request (GET) om de API te triggeren (GetAndPostWeerstationList & GetAndPostWeerberichtList)
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                
                // Authorisatie voor toegang in API
                string encodedString = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(ConfigurationManager.AppSettings["consoleUsername"].ToString() + ":" + ConfigurationManager.AppSettings["consolePassword"].ToString()));
                webRequest.Headers.Add("Authorization", "Basic " + encodedString);

                Console.WriteLine(DateTime.Now.ToString() + " | Done. Executing web response...");
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Something went wrong: " + e.ToString());
                return false;
            }
            
        }
    }
}
