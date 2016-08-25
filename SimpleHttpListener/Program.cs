using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServerConfig serverConfig;
            serverConfig = getServerConfig();

            string protocol = serverConfig.protocol;
            string port = serverConfig.port.ToString();

            string apiEndpoint = protocol + "://*:" + port + "/api/";
            string UiEndpoint = protocol + "://*:" + port + "/ui/";

            string[] uris = new string[2];
            uris[0] = apiEndpoint;
            uris[1] = UiEndpoint;

            SimpleListenerExample(uris);
        }

        // This example requires the System and System.Net namespaces.
        public static void SimpleListenerExample(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }

            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request. 
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                Console.WriteLine(request.RawUrl);
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.

                string responseString = "";
                // Return HTML page
                if (request.RawUrl.Split('?')[0].StartsWith("/ui"))
                {
                    using (StreamReader reader = new StreamReader("index.html"))
                    {
                        responseString = reader.ReadToEnd();
                    }
                }
                else if (request.RawUrl.Split('?')[0].StartsWith("/api")) {
                    responseString = "{\"name\": \"Ramtin\", \"age\": \"32\", \"city\": \"Berkeley\", \"request\": " + request.Url +"}";
                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            
            listener.Stop();
        }

        static WebServerConfig getServerConfig()
        {
            using (StreamReader reader = new StreamReader("webconfig.json"))
            {
                string jsonConfigString = reader.ReadToEnd();
                WebServerConfig serverConfig = JsonConvert.DeserializeObject<WebServerConfig>(jsonConfigString);
                return serverConfig;
            }
        }
    }


    public class WebServerConfig
    {
        public string protocol;
        public string hostname;
        public int port;
    }
}
