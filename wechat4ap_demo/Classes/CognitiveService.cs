using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace wechat4ap_demo.Classes
{
    //"{\"documents\":[{\"score\":0.6623393,\"id\":\"1\"}],\"errors\":[]}"
    public class CongnitiveResult
    {
        public docItem[] documents { get; set; }
    }

    public class docItem
    {
        public float score { get; set; }
        public int id { get; set; }
    }

    public class CognitiveService
    {
        static string ServiceBaseUri = "https://westus.api.cognitive.microsoft.com/";
        static string accountKey = "d2436d5b794b4c9fa9181b0ce6203a18";



        public static async Task<string> GetSentiment(string inputText)
        {
            string uri;
            string response;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ServiceBaseUri);

                    // Request headers.
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accountKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Request body. Insert your text data here in JSON format.
                    byte[] byteData = Encoding.UTF8.GetBytes("{\"documents\":[" +
                        "{\"id\":\"1\",\"text\":\"" + inputText + "\"}," + "]}");

                    //// Detect key phrases:
                    //var uri = "text/analytics/v2.0/keyPhrases";
                    //var response = await CallEndpoint(client, uri, byteData);
                    // Detect sentiment:
                    uri = "text/analytics/v2.0/sentiment";
                    response = await CallEndpoint(client, uri, byteData);
                    //Console.WriteLine("\nDetect sentiment response:\n" + response);

                    CongnitiveResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<CongnitiveResult>(response);

                    if (res.documents[0].score > 0.5)
                        return "\ue00e" + " good with score:" + res.documents[0].score.ToString();
                    else
                        return "\ue421" + " bad with score:" + res.documents[0].score.ToString();
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }


        public static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}