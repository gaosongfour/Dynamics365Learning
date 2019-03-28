using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System.Security;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace CommonHelper
{
    public class CrmServiceHelper
    {

        /// <summary>
        /// Get Crm OrganizationService Proxy
        /// </summary>
        /// <param name="config"></param>
        public static OrganizationServiceProxy GetOrganizationServiceProxy(ServerConfiguration config)
        {
            //add the following code, otherwise, we will receive an error message like medata contains a ref that can not be resoved
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            var serviceProxy = new OrganizationServiceProxy(config.OrganizationUri, config.HomeRealmUrl, config.Credentials, config.DeviceCredentials);
            serviceProxy.EnableProxyTypes();
            return serviceProxy;
        }

        public static CrmServiceClient GetCrmServiceClient(string crmConnString)
        {
            CrmServiceClient service = new CrmServiceClient(crmConnString);
            return service;
        }

        #region SecureString
        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        #endregion
    }

    /// <summary>
    /// Enum for CRM auth type
    /// </summary>
    public enum CrmAuthType
    {
        IFD, AD, Office365
    }

    ///<summary>
    ///Enum for CRM version
    /// </summary>
    public enum CrmVersion
    {
        CrmV8, CRMV9
    }


}
