using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class CrmWebAPIHelper
    {
        //private HttpClient httpClient;

        public string WebAPIServiceUrl { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string DomainName { get; private set; }

        public CrmWebAPIHelper(ServerConfiguration config = null)
        {
            if (config == null)
            {
                config = new ServerConfiguration();
                config.GetServerConfiguration();
            }

            WebAPIServiceUrl = config.WebAPIServiceUrl;
            UserName = config.UserName;
            Password = config.Password;
            DomainName = config.DomainName;
        }

        public async Task<string> SendGetRequestAsync(string query, bool formatted = false)
        {
            string result;
            HttpClientHandler crmHandler = new HttpClientHandler() { Credentials = new NetworkCredential(UserName, Password, DomainName) };
            HttpClient httpClient = new HttpClient(crmHandler, true);
            httpClient.Timeout = new TimeSpan(0, 0, 2);
            httpClient.BaseAddress = new Uri(WebAPIServiceUrl);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (formatted)
                httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=OData.Community.Display.V1.FormattedValue");

            var response = await httpClient.GetAsync(query, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
            }
            else
                throw new CrmHttpResponseException(response.Content);

            return result;
        }
    }
}
