using System;
using RestSharp;

namespace KiviTR.Common
{

    public class Request
    {
        public string SimpleGet(string url)
        {
            RestClient client = new RestClient(new Uri(url));
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }

}