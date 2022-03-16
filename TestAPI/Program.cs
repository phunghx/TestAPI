using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TestAPI
{
    public static class ElasticSearch
    {
        public static string ToJsonString(this JsonDocument jdoc)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public static HttpClient getClient()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            var client = new HttpClient(handler);
            var byteArray = Encoding.ASCII.GetBytes("elastic:GSbLU=X63NL6YRUGDVWF");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/json; charset=utf-8");
            return client;
        }
        public static async System.Threading.Tasks.Task<string> postDataAsync(JsonDocument doc)
        {
            var content = new StringContent(ElasticSearch.ToJsonString(doc),
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 "https://52.0.7.90:9200/logs-my_app-default/_doc?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            

            string data = @"  { ""@timestamp"": ""2099-05-06T16:21:15.000Z"",
                        ""event"": 
                           {""original"": 
                            ""192.0.2.42 - - [06 / May / 2099:16:21:15 + 0000] \""GET /images/bg.jpg HTTP/1.0\"" 200 24736""}}";
            using JsonDocument doc = JsonDocument.Parse(data);

            var result = ElasticSearch.postDataAsync(doc);
            Console.WriteLine(result.Result);
            
        }
    }
}
