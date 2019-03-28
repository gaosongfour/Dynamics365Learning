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
using CommonHelper;

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

            //load ComboBox from enum CrmVersion
            CbbCrmVersion.ItemsSource = Enum.GetValues(typeof(CrmVersion)).Cast<CrmVersion>();
            CbbCrmVersion.SelectedIndex = 1;

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
                var service = CrmServiceHelper.GetCrmServiceClient(config.CrmConnString);
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
            var crmVersion = CbbCrmVersion.SelectedValue.ToString().Contains("8") ? "8" : "9";
            var authType = CbbAuthType.SelectedIndex.ToString();
            var serverAddress = TbxCrmServer.Text;
            var port = TbxPort.Text;
            var orgName = TbxOrgName.Text;
            var userName = TbxUserName.Text;
            var userPassword = CrmServiceHelper.ConvertToUnsecureString(TbxPassword.SecurePassword);
            var userDomain = TbxDomain.Text;
            var useSSL = CbxUseSSL.IsChecked.HasValue ? CbxUseSSL.IsChecked.Value : false;

            var serverConfig = new ServerConfiguration();
            serverConfig.GetServerConfigurationFromUserInput(crmVersion, authType, serverAddress, port, orgName,
                useSSL, userName, userPassword, userDomain);

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
                LblCrmServerEx.Content = "ex: orgname.domainname.com(without https://)";
            }
            else if (CbbAuthType.SelectedIndex == 1) //AD
            {
                LblCrmServerEx.Content = "ex: servername";
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
