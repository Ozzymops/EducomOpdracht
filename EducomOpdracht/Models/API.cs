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
        public List<Weerstation> GetAllWeerstations()
        {
            string json = string.Empty;

            // Ophalen data uit API als json
            HttpWebRequest getWebRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["weerstationsUrl"]);
            getWebRequest.Method = "GET";
            getWebRequest.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse getWebResponse = (HttpWebResponse)getWebRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(getWebResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            JArray parsedJson = JArray.Parse(json);
            int weerstationCount = parsedJson.Count;

            List<Weerstation> weerstations = new List<Weerstation>();

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

            if (weerstations.Count != 0)
            {
                return weerstations;
            }

            return null;
        }

        public List<Weerbericht> GetAllWeerberichten()
        {
            string json = string.Empty;

            // Ophalen data uit API als json
            HttpWebRequest getWebRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["weerberichtenUrl"]);
            getWebRequest.Method = "GET";
            getWebRequest.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse getWebResponse = (HttpWebResponse)getWebRequest.GetResponse();
            using (StreamReader getStreamReader = new StreamReader(getWebResponse.GetResponseStream()))
            {
                json = getStreamReader.ReadToEnd();
            }

            JArray parsedJson = JArray.Parse(json);
            int weerberichtCount = parsedJson.Count;

            List<Weerbericht> weerberichten = new List<Weerbericht>();

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

            if (weerberichten.Count != 0)
            {
                return weerberichten;
            }

            return null;
        }
    }
}
