using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    public class Monitor : IMonitor
    {
        public SensorData getSensorData(string ghId)
        {
            string url = $"http://193.6.19.58:8181/greenhouse/{ghId}";
            SensorData sensorData;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required
            // encoding format.
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string JSON = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            sensorData = JsonConvert.DeserializeObject<SensorData>(JSON);

            return sensorData;
        }
    }
}
