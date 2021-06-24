using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class API
    {
        /// <summary>
        /// Haalt alle weerstations op uit de database.
        /// </summary>
        /// <returns>Een lijst van Weerstations</returns>
        public List<Weerstation> GetAllWeerstations()
        {
            string json = string.Empty;

            // Creëer een web request (GET) richting de API endpoint voor weerstations.
            HttpWebRequest getWebRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["weerstationsUrl"]);
            getWebRequest.Method = "GET";
            getWebRequest.ContentType = "application/x-www-form-urlencoded";

            // SSL certificate error fix, anders werkt het niet op IIS
            getWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // Creëer een web response om de request mee uit te voeren en data terug te krijgen om in te lezen.
            HttpWebResponse getWebResponse = (HttpWebResponse)getWebRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(getWebResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            // Parse de verkregen data zodat het bruikbaar is.
            JArray parsedJson = JArray.Parse(json);

            // Tel het aantal weerstations voor de for-loop.
            int weerstationCount = parsedJson.Count;

            List<Weerstation> weerstations = new List<Weerstation>();

            // Maak een nieuwe Weerstation object per json entry en plaats deze in een lijst.
            for (int i = 0; i < weerstationCount; i++)
            {
                Weerstation weerstation = new Weerstation(parsedJson.SelectToken("[" + i +"].stationId").Value<long>(),
                    parsedJson.SelectToken("[" + i + "].date").Value<DateTime>(),
                    parsedJson.SelectToken("[" + i + "].region").Value<string>(),
                    parsedJson.SelectToken("[" + i + "].name").Value<string>(),
                    parsedJson.SelectToken("[" + i + "].temperatureGc").Value<double>(),
                    parsedJson.SelectToken("[" + i + "].temperatureCm").Value<double>(),
                    parsedJson.SelectToken("[" + i + "].windspeedMs").Value<double>(),
                    parsedJson.SelectToken("[" + i + "].windspeedBf").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].humidity").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].airPressure").Value<double>());

                weerstations.Add(weerstation);
            }

            // Dubbelcheck of er überhaupt resultaten zijn. Zo ja, klaar!
            if (weerstations.Count != 0)
            {
                return weerstations;
            }

            return null;
        }

        /// <summary>
        /// Haalt alle weerberichten op uit de database.
        /// </summary>
        /// <returns>Een lijst van Weerberichten</returns>
        public List<Weerbericht> GetAllWeerberichten()
        {
            string json = string.Empty;

            // Creëer een web request (GET) richting de API endpoint voor weerberichten.
            HttpWebRequest getWebRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["weerberichtenUrl"]);
            getWebRequest.Method = "GET";
            getWebRequest.ContentType = "application/x-www-form-urlencoded";

            // SSL certificate error fix, anders werkt het niet op IIS
            getWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // Creëer een web response om de request mee uit te voeren en data terug te krijgen om in te lezen.
            HttpWebResponse getWebResponse = (HttpWebResponse)getWebRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(getWebResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            // Parse de verkregen data zodat het bruikbaar is.
            JArray parsedJson = JArray.Parse(json);

            // Tel het aantal weerberichten voor de for-loop.
            int weerberichtCount = parsedJson.Count;

            List<Weerbericht> weerberichten = new List<Weerbericht>();

            // Maak een nieuwe Weerbericht object per json entry en plaats deze in een lijst.
            for (int i = 0; i < weerberichtCount; i++)
            {
                Weerbericht weerbericht = new Weerbericht(parsedJson.SelectToken("[" + i + "].date").Value<DateTime>(),
                    parsedJson.SelectToken("[" + i + "].maxTemperature").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].minTemperature").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].windspeed").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].rainChance").Value<int>(),
                    parsedJson.SelectToken("[" + i + "].sunChance").Value<int>());

                weerberichten.Add(weerbericht);
            }

            // Dubbelcheck of er überhaupt resultaten zijn. Zo ja, klaar!
            if (weerberichten.Count != 0)
            {
                return weerberichten;
            }

            return null;
        }
    }
}
