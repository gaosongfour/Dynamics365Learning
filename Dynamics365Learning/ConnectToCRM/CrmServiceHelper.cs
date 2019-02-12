using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace ConnectToCRM
{
    public class CrmServiceHelper
    {
        /// <summary>
        /// Get Crm OrganizationService For IFD On-premise
        /// </summary>
        /// <param name="config"></param>
        public OrganizationServiceProxy GetServiceProxyIFD(ServerConfiguration config)
        {
            var serviceProxy = new OrganizationServiceProxy(config.OrganizationUri, config.HomeRealmUrl, config.Credentials, config.DeviceCredentials);
            serviceProxy.EnableProxyTypes();
            return serviceProxy;
        }
    }
}
