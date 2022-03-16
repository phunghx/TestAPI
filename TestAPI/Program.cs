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
        private static HttpClient client;
        private static string IP = "https://52.0.7.90:9200";
        private static string authen = "elastic:GSbLU=X63NL6YRUGDVWF";

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
            if (ElasticSearch.client is null)
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                ElasticSearch.client = new HttpClient(handler);
                var byteArray = Encoding.ASCII.GetBytes($"{ElasticSearch.authen}");
                ElasticSearch.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(byteArray));
                ElasticSearch.client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                    "application/json; charset=utf-8");
            }
            return ElasticSearch.client;
        }
        public static async System.Threading.Tasks.Task<string> postDataAsync(JsonDocument doc)
        {
            var content = new StringContent(ElasticSearch.ToJsonString(doc),
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/logs-my_app-default/_doc?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
        public static async System.Threading.Tasks.Task<string> postDataAsyncMulti(string[] docs)
        {
            string postdata = "";
            foreach (var doc in docs)
            {
                using JsonDocument doc2 = JsonDocument.Parse(doc);
                postdata += ElasticSearch.ToJsonString(doc2).Replace("\n","") + " ";
            }
            Console.WriteLine(postdata);
            var content = new StringContent(postdata,
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/logs-my_app-default/_bulk?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
        public static async System.Threading.Tasks.Task<string> getDataAsync(JsonDocument doc)
        {
            

            var content = new StringContent(ElasticSearch.ToJsonString(doc),
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/logs-my_app-default/_search?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {



            //string data = @"  { ""@timestamp"": ""2099-05-06T16:21:15.000Z"",
            //            ""event"": 
            //               {""original"": 
            //                ""192.0.2.42 - - [06 / May / 2099:16:21:15 + 0000] \""GET /images/bg.jpg HTTP/1.0\"" 200 24736""}}";
            //using JsonDocument doc = JsonDocument.Parse(data);

            //var result = ElasticSearch.postDataAsync(doc);
            //Console.WriteLine(result.Result);

            //string [] data ={ @"  {""create"": { }}",
            //             @"{ ""@timestamp"": ""2099-05-06T16:21:15.000Z"",
            //            ""event"": 
            //               {""original"": 
            //                ""192.0.2.42 - - [06 / May / 2099:16:21:15 + 0000] \""GET /images/bg.jpg HTTP/1.0\"" 200 24736""}}",
            //              @"  {""create"": { }}",
            //             @"{ ""@timestamp"": ""2099-06-06T16:21:15.000Z"",
            //            ""event"": 
            //               {""original"": 
            //                ""192.0.2.42 - - [06 / May / 2099:16:21:15 + 0000] \""GET /images/bg.jpg HTTP/1.0\"" 200 24736""}}"


            //           };




            //var result2 = ElasticSearch.postDataAsyncMulti(data);
            //Console.WriteLine(result2.Result);

            //string data = @"  { ""query"": {""match_all"":{}},
            //            ""sort"": [ 
            //               {""@timestamp"": 
            //                ""desc""}]}";
            //using JsonDocument doc = JsonDocument.Parse(data);

            //var result = ElasticSearch.getDataAsync(doc);
            //Console.WriteLine(result.Result);


            string data = @"{
                    ""query"": {
                        ""range"": {
                                    ""@timestamp"": {
                                        ""gte"": ""3099-05-05"",
                            ""lt"": ""4099-05-08""
                                    }
                                }
                            },
                      ""fields"": [
                        ""@timestamp""
                      ],
                      ""_source"": false,
                      ""sort"": [
                        {
                          ""@timestamp"": ""desc""
                        }
                      ]
                                                }
                    ";
            using JsonDocument doc = JsonDocument.Parse(data);
            var result = ElasticSearch.getDataAsync(doc);
            Console.WriteLine(result.Result);
        }
    }
}
