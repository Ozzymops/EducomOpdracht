using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EducomOpdrachtAPI.Models
{
    public class Database
    {
        public List<Weerstation> weerstations;
        public List<Weerbericht> weerberichten;

        /// <summary>
        /// Haalt weerstations op uit Buienradar-feed, verwerkt ze en plaatst ze in de 'weerstations' list.
        /// </summary>
        public void GetWeerstationsFromFeed()
        {
            string json = string.Empty;

            // WebRequest en WebResponse voor het ophalen van json data uit Buienradar-feed
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["buienradarUrl"]);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            Console.WriteLine(DateTime.Now.ToString() + " | Done. Executing web response...");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            // Data extractie uit json resultaat
            weerstations = new List<Weerstation>();
            weerberichten = new List<Weerbericht>();

            // Json parsing
            JObject parsedJson = JObject.Parse(json);
         
            // Aantal weerstations is onbekend, dus voor de loop moet een nummer bepaald worden - de Count van weerstations
            int weerstationCount = parsedJson.SelectToken("buienradarnl.weergegevens.actueel_weer.weerstations").Value<JArray>("weerstation").Count;

            // Weerstations: Loop door json resultaten heen en plaats resultaten in lijst
            for (int a = 0; a < weerstationCount; a++)
            {
                // Een string voor hergebruik (bespaart typen)
                string weerstationChain = "buienradarnl.weergegevens.actueel_weer.weerstations.weerstation[" + a.ToString() + "]";

                // De waarden die opgehaald worden
                double temperatureGc;
                double temperatureCm;
                double windspeedMs;
                int windspeedBf;
                int humidity;
                double airPressure;

                // Niet alle waarden bestaan bij alle weerstations, NULL kan dus wel eens terugkomen en de API laten crashen.
                // Daarom heb ik try/catch blokken geplaatst, om een placeholder getal (-999) terug te geven, wat later hardcoded staat om zo goed als n.v.t. te betekenen.
                #region Try/catch blokken
                try
                {
                    temperatureGc = parsedJson.SelectToken(weerstationChain + ".temperatuurGC").Value<double>();
                }
                catch
                {
                    temperatureGc = -999;
                }

                try
                {
                    temperatureCm = parsedJson.SelectToken(weerstationChain + ".temperatuur10cm").Value<double>();
                }
                catch
                {
                    temperatureCm = -999;
                }

                try
                {
                    windspeedMs = parsedJson.SelectToken(weerstationChain + ".windsnelheidMS").Value<double>();
                }
                catch
                {
                    windspeedMs = -999;
                }

                try
                {
                    windspeedBf = parsedJson.SelectToken(weerstationChain + ".windsnelheidBF").Value<int>();
                }
                catch
                {
                    windspeedBf = -999;
                }

                try
                {
                    humidity = parsedJson.SelectToken(weerstationChain + ".luchtvochtigheid").Value<int>();
                }
                catch
                {
                    humidity = -999;
                }

                try
                {
                    airPressure = parsedJson.SelectToken(weerstationChain + ".luchtdruk").Value<double>();
                }
                catch
                {
                    airPressure = -999;
                }
                #endregion

                // Bouw Weerstation-object met een constructor
                Weerstation weerstation = new Weerstation(parsedJson.SelectToken(weerstationChain + ".stationcode").Value<long>(),
                    parsedJson.SelectToken(weerstationChain + ".datum").Value<DateTime>(),
                    parsedJson.SelectToken(weerstationChain + ".stationnaam.@regio").Value<string>(),
                    parsedJson.SelectToken(weerstationChain + ".stationnaam.#text").Value<string>(),
                    temperatureGc,
                    temperatureCm,
                    windspeedMs,
                    windspeedBf,
                    humidity,
                    airPressure);

                // Plaats Weerstation-object in de lijst
                weerstations.Add(weerstation);
            }
        }

        /// <summary>
        /// Haalt weerberichten op uit Buienradar-feed, verwerkt ze en plaatst ze in de 'weerberichten' list.
        /// </summary>
        public void GetWeerberichtenFromFeed()
        {
            string json = string.Empty;

            // WebRequest en WebResponse voor het ophalen van json data uit Buienradar-feed
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["buienradarUrl"]);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            Console.WriteLine(DateTime.Now.ToString() + " | Done. Executing web response...");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            // Data extractie uit json resultaat
            weerstations = new List<Weerstation>();
            weerberichten = new List<Weerbericht>();

            // Json parsing
            JObject parsedJson = JObject.Parse(json);

            // Weerberichten: Loop door json resultaten heen en plaats resultaten in lijst
            // Aantal weerberichten is WEL bekend, want Buienradar heeft altijd tot op vijf dagen verder een meerdaagse voorspelling klaar staan
            // Dus er hoeft maar vijf keer geloopt te worden - er wordt bij 1 gestart omdat de json resultaten ook bij id: 1 starten.
            for (int b = 1; b < 6; b++)
            {
                // Een string voor hergebruik (bespaart typen)
                string weerberichtChain = "buienradarnl.weergegevens.verwachting_meerdaags.dag-plus" + b.ToString();

                // Bouw Weerbericht-object met een constructor
                Weerbericht weerbericht = new Weerbericht(DateTime.Parse(parsedJson.SelectToken(weerberichtChain + ".datum").Value<string>(), new System.Globalization.CultureInfo("nl-NL")),
                    parsedJson.SelectToken(weerberichtChain + ".maxtemp").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".mintemp").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".windkracht").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".kansregen").Value<int>(),
                    parsedJson.SelectToken(weerberichtChain + ".kanszon").Value<int>());

                // Plaats Weerbericht-object in de lijst
                weerberichten.Add(weerbericht);
            }
        }
    }
}
