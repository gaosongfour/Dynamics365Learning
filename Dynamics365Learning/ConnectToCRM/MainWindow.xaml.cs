using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Net;

namespace ConnectToCRM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitUserControl();
        }

        private void InitUserControl()
        {
            //Load ComboBox from enum AuthType
            CbbAuthType.ItemsSource = Enum.GetValues(typeof(CrmAuthType)).Cast<CrmAuthType>();
            CbbAuthType.SelectedIndex = 0;

            //Hidden Main Tab
            TabMain.Visibility = Visibility.Hidden;

            //AuthType
            OnChangeAuthType();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = GetServerConfiguration();
                var service = CrmServiceHelper.GetOrganizationServiceProxy(config);

                WhoAmIRequest request = new WhoAmIRequest();
                var response = (WhoAmIResponse)service.Execute(request);

                CrmConnectionSuccessful(response.UserId.ToString());

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCrmServiceClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = GetServerConfiguration();
                var service = CrmServiceHelper.GetCrmServiceClient(config.crmConnString);
                if (service != null && service.IsReady)
                {
                    WhoAmIRequest request = new WhoAmIRequest();
                    var response = (WhoAmIResponse)service.Execute(request);

                    CrmConnectionSuccessful(response.UserId.ToString());
                }
                else
                {
                    throw new Exception(service.LastCrmError);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Get server configuration from user input 
        /// </summary>
        private ServerConfiguration GetServerConfiguration()
        {
            var authType = CbbAuthType.SelectedIndex;
            var serverAddress = TbxCrmServer.Text;
            var port = TbxPort.Text;
            var userName = TbxUserName.Text;
            var userPassword = TbxPassword.SecurePassword;
            var userDomain = TbxDomain.Text;
            var useSSL = CbxUseSSL.IsChecked.HasValue ? CbxUseSSL.IsChecked.Value : false;

            var serverConfig = new ServerConfiguration();
            if (authType == 0) // IFD
            {
                serverAddress = serverAddress.ToLower().Replace("https://", "").Replace("http://", "");
                if (string.IsNullOrWhiteSpace(port))
                    serverConfig.OrganizationUri = new Uri(string.Format("https://{0}/XRMServices/2011/Organization.svc", serverAddress));
                else
                    serverConfig.OrganizationUri = new Uri(string.Format("https://{0}:{1}/XRMServices/2011/Organization.svc", serverAddress, port));

                serverConfig.Credentials = new ClientCredentials();
                serverConfig.Credentials.UserName.UserName = string.Format("{0}@{1}", userName, userDomain);
                serverConfig.Credentials.UserName.Password = CrmServiceHelper.ConvertToUnsecureString(userPassword);

                //Optional, construct the crmConnString if needed
                var orgName = serverAddress.Substring(0, serverAddress.IndexOf("."));
                serverConfig.crmConnString = string.Format("AuthType=IFD; Url=https://{0}/{1}; Domain={2};Username={3}; Password={4}"
                   , serverAddress, orgName, userDomain, serverConfig.Credentials.UserName.UserName, serverConfig.Credentials.UserName.Password);

            }
            else if (authType == 1) //AD on-premise, TO TEST
            {
                var http = "http" + (useSSL ? "s" : null);
                if (string.IsNullOrWhiteSpace(port))
                    serverConfig.OrganizationUri = new Uri(string.Format("{0}://{1}/XRMServices/2011/Organization.svc", http, serverAddress));
                else
                    serverConfig.OrganizationUri = new Uri(string.Format("{0}://{1}:{2}/XRMServices/2011/Organization.svc", http, serverAddress, port));

                serverConfig.Credentials = new ClientCredentials();
                serverConfig.Credentials.Windows.ClientCredential = new NetworkCredential(userName, userPassword, userDomain);
                //Or Use default windows crendential
                //serverConfig.Credentials.Windows.ClientCredential = (NetworkCredential)CredentialCache.DefaultCredentials;

            }
            else if (authType == 2)//online office365 not availiable
            {
                //To do
            }
            else
                throw new Exception("Invalid AuthType");

            return serverConfig;
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            TabConnect.IsSelected = true;
            TabConnect.Visibility = Visibility.Visible;

            TabMain.Visibility = Visibility.Hidden;
            TbxUserInfo.Text = null;
        }

        private void CbbAuthType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnChangeAuthType();
        }

        private void OnChangeAuthType()
        {
            if (CbbAuthType.SelectedIndex == 0) //IFD
            {
                LblCrmServerEx.Content= "ex: orgname.domainname.com(without https://)";
            }
            else if (CbbAuthType.SelectedIndex == 1) //AD
            {
                LblCrmServerEx.Content = "ex: servername/orgname";
            }
            else
            {
                LblCrmServerEx.Content = "ex: to complete";
            }
        }

        private void CrmConnectionSuccessful(string message)
        {
            TabConnect.Visibility = Visibility.Hidden;

            TabMain.Visibility = Visibility.Visible;
            TabMain.IsSelected = true;
            TbxUserInfo.Text = "Connect to CRM OK " + message;
        }

    }
}
