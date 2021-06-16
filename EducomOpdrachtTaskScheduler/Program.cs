using System;
using System.IO;
using System.Net;
using System.Text;

namespace EducomOpdrachtTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // API URL om uit te voeren, in dit geval de update database functie.
            Console.WriteLine("Console application started - updating database...");
            string apiUrl = string.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["APIURL"]);
            string result = UpdateDatabase(apiUrl);

            Console.WriteLine("Finished, press any key to end application...");
            Console.Read(); // verwijder later, alleen voor debug
        }

        public static string UpdateDatabase(string apiUrl)
        {
            string result = string.Empty;

            //try
            //{
                Console.WriteLine("Web request...");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                webRequest.Method = "GET"; // wordt later UPDATE?
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("Username", "abc");
                webRequest.Headers.Add("Password", "def");

                Console.WriteLine("Web response...");
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Encoding encoding = Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), encoding);

                result = responseStream.ReadToEnd();
                webResponse.Close();
                Console.WriteLine("Response acquired: " + result);
            //}
            //catch
            //{
            //    Console.WriteLine("Failed.");
            //    result = "Error.";
            //}

            return result;
        }
    }
}
