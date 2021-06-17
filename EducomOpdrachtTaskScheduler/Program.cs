using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using EducomOpdrachtTaskScheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EducomOpdrachtTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // API URL om uit te voeren, in dit geval de update database functie.
            Console.WriteLine(DateTime.Now.ToString() + " | Console application started.");
            string result = UpdateDatabase(System.Configuration.ConfigurationManager.AppSettings["weerstationsUrl"], System.Configuration.ConfigurationManager.AppSettings["weerberichtenUrl"], System.Configuration.ConfigurationManager.AppSettings["buienradarUrl"]);
            Console.WriteLine(DateTime.Now.ToString() + " | Finished, press any key to end application...");
            Console.Read(); // verwijder later, alleen voor debug
        }

        public static string UpdateDatabase(string weerstationsUrl, string weerberichtenUrl, string buienradarUrl)
        {
            string json = string.Empty;

            // Ophalen data buienradar door middel van een web request en response (benadert de API van buienradar)
            Console.WriteLine(DateTime.Now.ToString() + " | Creating web request [GET]...");
            HttpWebRequest getWebRequest = (HttpWebRequest)WebRequest.Create(buienradarUrl);
            getWebRequest.Method = "GET";
            getWebRequest.ContentType = "application/x-www-form-urlencoded";

            Console.WriteLine(DateTime.Now.ToString() + " | Done. Executing web response...");
            HttpWebResponse getWebResponse = (HttpWebResponse)getWebRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(getWebResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            Console.WriteLine(DateTime.Now.ToString() + " | Done. Extracting relevant data from result...");

            // Haalt relevante data uit resultaat
            List<Weerstation> weerstations = new List<Weerstation>();
            List<Weerbericht> weerberichten = new List<Weerbericht>();

            JObject parsedJson = JObject.Parse(json);
            int weerstationCount = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations").Value<JArray>("weerstation").Count;

            Console.WriteLine(DateTime.Now.ToString() + " | Processing " + weerstationCount.ToString() + " results...");

            // Loop dat alle data verwerkt en in lijsten stopt
            for (int count = 0; count < weerstationCount; count++)
            {
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
            }

            // (misschien bad practice? maar werkt voor nu)
            // Loop door de lijst en voer POST functies uit
            foreach (Weerstation weerstation in weerstations)
            {
                // Serialiseer object in lijst zodat het verstuurd kan worden
                string weerstationJson = JsonConvert.SerializeObject(weerstation);

                // Verwijs naar POST functie
                if (!DoPost(weerstationsUrl, weerstationJson))
                {
                    Console.WriteLine(DateTime.Now.ToString() + " | Weatherstation operation aborted.");
                    break;
                }
            }

            foreach (Weerbericht weerbericht in weerberichten)
            {
                // Serialiseer object in lijst zodat het verstuurd kan worden
                string weerberichtJson = JsonConvert.SerializeObject(weerbericht);

                // Verwijs naar POST functie
                if (!DoPost(weerberichtenUrl, weerberichtJson))
                {
                    Console.WriteLine(DateTime.Now.ToString() + " | Weather forecast operation aborted.");
                    break;
                }
            }

            return string.Empty;
        }

        public static bool DoPost(string url, string json)
        {
            try
            {
                // POST functie richting API
                HttpWebRequest postWebRequest = (HttpWebRequest)WebRequest.Create(url);
                postWebRequest.ContentType = "application/json; charset=utf-8";
                postWebRequest.Method = "POST";

                // Authorisatie voor toegang tot POST methode in API
                string encodedString = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(ConfigurationManager.AppSettings["consoleUsername"].ToString() + ":" + ConfigurationManager.AppSettings["consolePassword"].ToString()));
                postWebRequest.Headers.Add("Authorization", "Basic " + encodedString);

                using (StreamWriter streamWriter = new StreamWriter(postWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse postWebResponse = (HttpWebResponse)postWebRequest.GetResponse();
                using (StreamReader postStreamReader = new StreamReader(postWebResponse.GetResponseStream()))
                {
                    string response = postStreamReader.ReadToEnd();
                    if (response != null && response != string.Empty)
                    {
                        Console.WriteLine(response);
                    }
                    else
                    {
                        Console.WriteLine("Operation failed, no authorisation.");
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString() + " | Something went wrong: " + e.Message.ToString());
                return false;
            }
            
        }
    }
}
