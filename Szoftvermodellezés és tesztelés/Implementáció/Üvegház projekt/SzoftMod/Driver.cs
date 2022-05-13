using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    public class Driver : IDriver
    {
        public int sendCommand(Greenhouse gh, string token, double boilerValue, double sprinklerValue)
        {
            string url = $"http://193.6.19.58:8181/greenhouse/{token}";
            string ghId = gh.ghId;
            string boilerCommand;
            string sprinklerCommand;
            int response = 0;
            if (boilerValue == 0 && sprinklerValue == 0)
            {
                boilerCommand = "";
                sprinklerCommand = "";
            }
            else if (boilerValue == 0)
            {
                Math.Round(sprinklerValue, 2);
                int int_boilderValue = 0;
                int int_sprinklerValue = Convert.ToInt32(sprinklerValue);
                boilerCommand = $"bup{int_boilderValue}c";
                sprinklerCommand = $"son{int_sprinklerValue}l";
            }
            else if (sprinklerValue == 0)
            {
                int int_boilderValue = Convert.ToInt32(boilerValue);
                int int_sprinklerValue = 0;
                boilerCommand = $"bup{int_boilderValue}c";
                sprinklerCommand = $"son{int_sprinklerValue}l";

            }
            else
            {
                Math.Round(sprinklerValue, 2);
                int int_boilderValue = Convert.ToInt32(boilerValue);
                int int_sprinklerValue = Convert.ToInt32(sprinklerValue);
                boilerCommand = $"bup{int_boilderValue}c";
                sprinklerCommand = $"son{int_sprinklerValue}l";
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = "POST";
            
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = @"{'ghId':'" + ghId + "','boilerCommand':'" + boilerCommand + "','sprinklerCommand':'" + sprinklerCommand + "'}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                response = Convert.ToInt32(result);
            }

            return response;
        }
    }
}
