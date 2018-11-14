using EasyNote.Integration.EasyNoteAPI.Exceptions;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Net;

namespace EasyNote.Integration.EasyNoteAPI.Gateway
{
    internal interface IAPIGateway
    {
        string ExecuteApiQuery(string endpoint,
                           Method httpMethod,
                           HttpStatusCode expectedStatusCode,
                            string token = null);
        string ExecuteAPICommand(string endpoint,
                            Method httpMethod,
                            object body,
                            HttpStatusCode expectedStatusCode,
                            string token = null);
        T ExecuteApiQuery<T>(string endpoint,
                           Method httpMethod,
                           HttpStatusCode expectedStatusCode,
                            string token = null);
        T ExecuteAPICommand<T>(string endpoint,
                            Method httpMethod,
                            object body,
                            HttpStatusCode expectedStatusCode,
                            string token = null);
    }

    internal class APIGateway : IAPIGateway
    {
        private readonly IRestClient client;

        public APIGateway()
        {
            var apiURL = ConfigurationManager.AppSettings["API_URL"];
            this.client = new RestClient(apiURL);
        }
        public string ExecuteApiQuery(string endpoint,
                           Method httpMethod,
                           HttpStatusCode expectedStatusCode,
                            string token = null)
        {
            return ExecuteAPICommand(endpoint, httpMethod, null, expectedStatusCode, token);
        }

        public T ExecuteApiQuery<T>(string endpoint,
                          Method httpMethod,
                          HttpStatusCode expectedStatusCode,
                            string token = null)
        {
            var response = ExecuteAPICommand(endpoint, httpMethod, null, expectedStatusCode, token);
            var deserialized = JsonConvert.DeserializeObject<T>(response);

            return deserialized;
        }

        public T ExecuteAPICommand<T>(string endpoint,
                            Method httpMethod,
                            object body,
                            HttpStatusCode expectedStatusCode,
                            string token = null)
        {
            var response = ExecuteAPICommand(endpoint, httpMethod, body, expectedStatusCode);

            var deserialized = JsonConvert.DeserializeObject<T>(response);

            return deserialized;
        }

        public string ExecuteAPICommand(string endpoint,
                            Method httpMethod,
                            object body,
                            HttpStatusCode expectedStatusCode,
                            string token = null)
        {
            var request = new RestRequest(endpoint, httpMethod);

            if (body != null && httpMethod != Method.GET)
                request.AddJsonBody(body);

            if(!string.IsNullOrWhiteSpace(token))
                request.AddHeader("Authorization", "Bearer " + token);

            var response = client.Execute(request);

            if (response.StatusCode != expectedStatusCode)
                throw new ServerResponseException(response.StatusCode);

            return response.Content;
        }
    }
}
