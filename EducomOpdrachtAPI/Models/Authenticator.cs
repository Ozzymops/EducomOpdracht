using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace EducomOpdrachtAPI.Models
{
    public class Authenticator
    {
        /// <summary>
        /// Checkt of gegeven request header de juiste strings bevat voor username en password, zoals gegeven in app/web.config
        /// </summary>
        /// <param name="requestHeader">Request header inclusief username/password</param>
        /// <returns></returns>
        public bool Authenticate(string requestHeader)
        {
            // Check of header niet leeg is
            if (requestHeader != null && requestHeader != string.Empty)
            {
                var authenticationHeader = AuthenticationHeaderValue.Parse(requestHeader);

                if (authenticationHeader != null
                    && authenticationHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                    && authenticationHeader.Parameter != null)
                {
                    // Decodeer username & password combo
                    var credentialPair = Encoding.ASCII.GetString(Convert.FromBase64String(authenticationHeader.Parameter));
                    var credentials = credentialPair.Split(new char[] { ':' }, StringSplitOptions.None);

                    string username = credentials[0];
                    string password = credentials[1];

                    // Username en password staan in web.config. Dit is puur en alleen zodat de console app een vorm van authenticatie heeft,
                    // en niet iedereen zomaar wat kan POSTen of PUTen - alleen de console app en website!
                    if (username == ConfigurationManager.AppSettings["consoleUsername"] && password == ConfigurationManager.AppSettings["consolePassword"])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checkt of gegeven request header de juiste strings bevat voor username en password, zoals gegeven in app/web.config
        /// </summary>
        /// <param name="loginData">Array met username/password</param>
        /// <returns></returns>
        public bool Authenticate(string[] loginData)
        {
            // Check of array niet leeg is
            if (loginData.Length != 0)
            {
                // Username en password staan in web.config. Dit is puur en alleen zodat de console app een vorm van authenticatie heeft,
                // en niet iedereen zomaar wat kan POSTen of PUTen - alleen de console app en website!
                if (loginData[0] == ConfigurationManager.AppSettings["consoleUsername"] && loginData[1] == ConfigurationManager.AppSettings["consolePassword"])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
