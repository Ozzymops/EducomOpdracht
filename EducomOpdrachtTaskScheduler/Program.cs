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
            // API URL om uit te voeren, in dit geval de update database functie.
            Console.WriteLine(DateTime.Now.ToString() + " | Console application started.");
            string result = UpdateDatabase(System.Configuration.ConfigurationManager.AppSettings["weerstationsUrl"], System.Configuration.ConfigurationManager.AppSettings["weerberichtenUrl"], System.Configuration.ConfigurationManager.AppSettings["buienradarUrl"]);
            Console.WriteLine(DateTime.Now.ToString() + " | Finished, console will close in 5 seconds...");
            Thread.Sleep(5000);
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

            Console.WriteLine(DateTime.Now.ToString() + " | Processing " + weerstationCount.ToString() + " weather stations...");

            // Loop dat alle data verwerkt en in lijsten stopt
            #region Weerstation weerberichten actueel
            for (int count = 0; count < weerstationCount; count++)
            {
                string weerstationChain = "buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + count.ToString() + "]";
                Weerstation weerstation = new Weerstation(parsedJson.SelectToken(weerstationChain + ".stationcode").Value<long>(),
                    parsedJson.SelectToken(weerstationChain + ".stationnaam.@regio").Value<string>(),
                    parsedJson.SelectToken(weerstationChain + ".stationnaam.#text").Value<string>());

                int tempTemperature;
                int tempHumidity;
                int tempAirPressure;

                try
                {
                    tempTemperature = (int)Math.Ceiling(parsedJson.SelectToken(weerstationChain + ".temperatuurGC").Value<double>());
                    tempHumidity = parsedJson.SelectToken(weerstationChain + ".luchtvochtigheid").Value<int>();
                    tempAirPressure = (int)Math.Ceiling(parsedJson.SelectToken(weerstationChain + "luchtdruk").Value<double>());
                }
                catch
                {
                    tempTemperature = -999;
                    tempHumidity = -999;
                    tempAirPressure = -999;
                }

                Weerbericht weerbericht = new Weerbericht(parsedJson.SelectToken(weerstationChain + ".datum").Value<DateTime>(),
                    weerstation.Id,
                    tempTemperature,
                    tempHumidity,
                    tempAirPressure);

                weerstations.Add(weerstation);
                weerberichten.Add(weerbericht);
            }
            #endregion

            #region Weerbericht 5-daags
            Console.WriteLine(DateTime.Now.ToString() + " | Processing forecast of +5 days...");

            // Haalt data op voor de meerdaagse verwachting
            for (int meerdaagseCount = 1; meerdaagseCount < 6; meerdaagseCount++)
            {
                string weerberichtChain = "buienradarnl.weergegevens.verwachting_meerdaags.dag-plus" + meerdaagseCount.ToString();
                Weerbericht weerbericht = new Weerbericht(DateTime.Parse(parsedJson.SelectToken(weerberichtChain + ".datum").Value<string>(), new System.Globalization.CultureInfo("nl-NL")),
                    parsedJson.SelectToken(weerberichtChain + ".maxtemp").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".mintemp").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".kansregen").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".kanszon").Value<int>());

                weerberichten.Add(weerbericht);
            }
            #endregion

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

                // SSL certificate error fix
                postWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

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
                Console.WriteLine(DateTime.Now.ToString() + " | Something went wrong: " + e.ToString());
                return false;
            }
            
        }
    }
}
