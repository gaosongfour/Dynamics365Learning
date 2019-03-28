using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.Configuration;
using System.Net;

namespace CommonHelper
{

    public class ServerConfiguration
    {
        /// <summary>
        /// Crm version "8" or "9"
        /// </summary>
        public string CrmVersion { get; set; }
        /// <summary>
        /// Crm AuthType "0"=IFD "1"=AD,"2"=online office365 not availiable
        /// </summary>
        public string AuthType { get; set; }
        public string ServerAddress { get; set; }
        public string PortNumber { get; set; }
        public string OrgName { get; set; }
        /// <summary>
        /// https or not, 0=NO otherwise Yes
        /// </summary>
        public bool UseSSL { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }

        public Uri DiscoveryUri { get; set; }
        public Uri OrganizationUri { get; set; }
        public Uri HomeRealmUrl { get; set; }
        public ClientCredentials Credentials { get; set; }
        public ClientCredentials DeviceCredentials { get; set; }

        /// <summary>
        /// Complete ConnectionString for CrmClient
        /// </summary>
        public string CrmConnString { get; set; }

        public ServerConfiguration()
        {
            HomeRealmUrl = null;
            DeviceCredentials = null;
            Credentials = null;
        }

        /// <summary>
        /// Get Configuration form App.config or WebConfig
        /// </summary>
        public void GetServerConfiguration()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("CrmVersion"))
                CrmVersion = ConfigurationManager.AppSettings["CrmVersion"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("AuthType"))
                AuthType = ConfigurationManager.AppSettings["AuthType"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerAddress"))
                ServerAddress = ConfigurationManager.AppSettings["ServerAddress"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("PortNumber"))
                PortNumber = ConfigurationManager.AppSettings["PortNumber"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("OrgName"))
                OrgName = ConfigurationManager.AppSettings["OrgName"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("UseSSL"))
                UseSSL = Boolean.Parse(ConfigurationManager.AppSettings["UseSSL"].ToString());
            if (ConfigurationManager.AppSettings.AllKeys.Contains("UserName"))
                UserName = ConfigurationManager.AppSettings["UserName"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("Password"))
                Password = ConfigurationManager.AppSettings["Password"].ToString();
            if (ConfigurationManager.AppSettings.AllKeys.Contains("DomainName"))
                DomainName = ConfigurationManager.AppSettings["DomainName"].ToString();

            PrepareServerConfiguration();

        }

        /// <summary>
        /// Get Configuration from User Input, command line or UI
        /// </summary>
        public void GetServerConfigurationFromUserInput(string crmVersion, string authType, string serverAddress, string portNumber,
             string orgName, bool useSSl, string userName, string password, string domainName)
        {
            CrmVersion = crmVersion;
            AuthType = authType;
            ServerAddress = serverAddress;
            PortNumber = portNumber;
            OrgName = orgName;
            UseSSL = useSSl;
            UserName = userName;
            Password = password;
            DomainName = domainName;

            PrepareServerConfiguration();
        }


        private void PrepareServerConfiguration()
        {
            
            if (AuthType == "0") // IFD
            {
                ServerAddress = ServerAddress.ToLower().Replace("https://", "").Replace("http://", "");
                if (string.IsNullOrWhiteSpace(PortNumber))
                    OrganizationUri = new Uri(string.Format("https://{0}/XRMServices/2011/Organization.svc", ServerAddress));
                else
                    OrganizationUri = new Uri(string.Format("https://{0}:{1}/XRMServices/2011/Organization.svc", ServerAddress, PortNumber));

                Credentials = new ClientCredentials();
                Credentials.UserName.UserName = string.Format("{0}@{1}", UserName, DomainName);
                Credentials.UserName.Password = Password;

                //Optional, construct the crmConnString if needed
                CrmConnString = string.Format("AuthType=IFD; Url=https://{0}/{1}; Domain={2};Username={3}; Password={4}"
                   , ServerAddress, OrgName, DomainName, Credentials.UserName.UserName, Credentials.UserName.Password);

            }
            else if (AuthType == "1") //AD on-premise, ex: http://petitserver:5555/KDI/XRMServices/2011/Organization.svc
            {
                var http = "http" + (UseSSL ? "s" : null);
                if (!string.IsNullOrWhiteSpace(PortNumber))
                    OrganizationUri = new Uri(string.Format("{0}://{1}:{2}/{3}/XRMServices/2011/Organization.svc", http, ServerAddress, PortNumber,OrgName));
                else
                    OrganizationUri = new Uri(string.Format("{0}://{1}/{2}/XRMServices/2011/Organization.svc", http, ServerAddress,OrgName));

                Credentials = new ClientCredentials();
                Credentials.Windows.ClientCredential = new NetworkCredential(UserName, Password, DomainName);
                //Or Use default windows crendential
                //Credentials.Windows.ClientCredential = (NetworkCredential)CredentialCache.DefaultCredentials;

            }
            else if (AuthType == "2")//online office365 not availiable
            {
                //To do
            }
            else
                throw new Exception("Invalid AuthType");
            
        }

    }
}

