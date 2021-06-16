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
            int weerstationCount = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations").Value<JArray>("weerstation").Count;
            Console.WriteLine(weerstationCount.ToString() + " weerstations gevonden.");

            // Loop dat alle data verwerkt en in lijsten stopt
            for (int count = 0; count < weerstationCount; count++)
            {
                Console.WriteLine("----------");

                Weerstation weerstation = new Weerstation();
                weerstation.Id = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].stationcode").Value<long>();
                weerstation.Name = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].stationnaam.#text").Value<string>();
                weerstation.Region = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].stationnaam.@regio").Value<string>();

                Weerbericht weerbericht = new Weerbericht();
                weerbericht.StationId = weerstation.Id;
                weerbericht.Date = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].datum").Value<DateTime>();

                // Niet alle weerstations hebben een thermometer, dus moet er een try/catch blok staan om de uitzonderingen op te vangen
                try
                {
                    weerbericht.Temperature = (int)Math.Ceiling(parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].temperatuurGC").Value<double>());
                }
                catch
                {
                    weerbericht.Temperature = -999;
                } 

                // Hetzelfde geldt voor de luchtvochtigheid en luchtdruk
                try
                {
                    weerbericht.Humidity = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].luchtvochtigheid").Value<int>();
                }
                catch
                {
                    weerbericht.Humidity = -999;
                }

                try
                {
                    weerbericht.AirPressure = (int)Math.Ceiling(parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count + "].luchtdruk").Value<double>());
                }
                catch
                {
                    weerbericht.AirPressure = -999;
                }

                weerstations.Add(weerstation);
                weerberichten.Add(weerbericht);

                Console.WriteLine(count.ToString() + " | " + weerstation.Id + ": " + weerstation.Name + ", " + weerstation.Region);
                Console.WriteLine(weerbericht.StationId.ToString() + " - Weer rondom " + weerbericht.Date.ToString() + ": " + weerbericht.Temperature.ToString() + "°C, luchtvochtigheid van " + weerbericht.Humidity.ToString() + " met een luchtdruk van " + weerbericht.AirPressure.ToString() );
            }
            
            // stuur door naar api 

            return string.Empty;
        }
    }
}
