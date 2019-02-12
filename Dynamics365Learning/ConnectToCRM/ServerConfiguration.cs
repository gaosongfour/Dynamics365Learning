using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;

namespace ConnectToCRM
{

    public class ServerConfiguration
    {
        public string ServerAddress { get; set; }
        public string OrganizationName { get; set; }
        public Uri DiscoveryUri { get; set; }
        public Uri OrganizationUri { get; set; }
        public Uri HomeRealmUrl { get; set; }
        public ClientCredentials Credentials { get; set; }
        public ClientCredentials DeviceCredentials { get; set; }
        public bool UserSSL { get; set; }

        public ServerConfiguration()
        {
            HomeRealmUrl = null;
            DeviceCredentials = null;
            Credentials = null;
        }
    }
}
