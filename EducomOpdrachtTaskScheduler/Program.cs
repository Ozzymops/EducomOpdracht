using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using EducomOpdrachtTaskScheduler.Models;
using Newtonsoft.Json.Linq;

namespace EducomOpdrachtTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // API URL om uit te voeren, in dit geval de update database functie.
            Console.WriteLine("Console application started.");
            string result = UpdateDatabase(System.Configuration.ConfigurationManager.AppSettings["apiUrl"], System.Configuration.ConfigurationManager.AppSettings["buienradarUrl"]);
            Console.WriteLine("Finished, press any key to end application...");
            Console.Read(); // verwijder later, alleen voor debug
        }

        public static string UpdateDatabase(string apiUrl, string buienradarUrl)
        {
            string json = string.Empty;

            // Ophalen data buienradar door middel van een web request en response (benadert de API van buienradar)
            Console.WriteLine("Creating web request...");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(buienradarUrl);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            Console.WriteLine("Done. Creating web response...");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), encoding);

            json = responseStream.ReadToEnd();
            webResponse.Close();
            Console.WriteLine("Done. Extracting relevant data from result...");

            // Haalt relevante data uit resultaat
            List<Weerstation> weerstations = new List<Weerstation>();
            List<Weerbericht> weerberichten = new List<Weerbericht>();

            JObject parsedJson = JObject.Parse(json);
            // int weerstationCount = parsedJson.Value<JArray>("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation").Value<JArray>("tags").Count;
            int weerstationCount = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations").Value<JArray>("weerstation").Count;
            Console.WriteLine(weerstationCount.ToString() + " weerstations gevonden.");

            for (int count = 0; count < weerstationCount; count++)
            {
                Weerstation weerstation = new Weerstation();

                weerstation.Id = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].stationcode").Value<long>();
                Console.WriteLine(count.ToString() + " | " + weerstation.Id);
            }
            

            

            return string.Empty;
        }
    }
}
