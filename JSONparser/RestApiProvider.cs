using RestSharp;

namespace JSONparser
{
    internal class RestApiProvider
    {
        RestClient client;

        public RestApiProvider(string baseUrl)
        {
            client = new RestClient(baseUrl);
        }

        public IRestResponse<T> GetData<T>(string resource)
        {
            var request = new RestRequest(resource);
            return client.Execute<T>(request);
        }
    }
}
