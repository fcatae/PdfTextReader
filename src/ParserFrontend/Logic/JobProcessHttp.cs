using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class JobProcessHttp
    {
        HttpClient _client;
        string _sitename;

        HttpClient GetClient()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _sitename = Environment.GetEnvironmentVariable("APPSETTING_WEBSITE_SITE_NAME")?.TrimEnd('/');
                Console.WriteLine("JobProcessHttp: URL = " + _sitename);
            }

            return _client;
        }

        public void Process(string name)
        {
            Console.WriteLine($"JobProcessHttp: Process Start [{name}]");
            GetClient().PostAsync($"http://{_sitename}.azurewebsites.net/job/{name}", new StringContent("")).Wait();
            Console.WriteLine($"JobProcessHttp: Process End [{name}]");
        }
    }
}
