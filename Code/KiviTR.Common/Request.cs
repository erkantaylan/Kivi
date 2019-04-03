using System;
using System.Threading.Tasks;
using RestSharp;

namespace KiviTR.Common
{
    public class Request
    {
        public string SimpleGet(string url)
        {
            var client = new RestClient(new Uri(url));
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public Task<string> SimpleGetAsync(string url)
        {
            return Task.Factory.StartNew(() => SimpleGet(url));
        }
    }
}