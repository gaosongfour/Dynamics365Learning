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
using System.ServiceModel.Description;
using Microsoft.Crm.Sdk.Messages;

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
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = GetServerConfiguration();
                var service = CrmServiceHelper.GetOrganizationServiceProxy(config);

                WhoAmIRequest request = new WhoAmIRequest();
                var response = (WhoAmIResponse)service.Execute(request);

                TabConnect.Visibility = Visibility.Hidden;
                TbxUserInfo.Text = response.UserId.ToString();

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
                if (string.IsNullOrWhiteSpace(port))
                    serverConfig.OrganizationUri = new Uri(string.Format("https://{0}/XRMServices/2011/Organization.svc", serverAddress));
                else
                    serverConfig.OrganizationUri = new Uri(string.Format("https://{0}:{1}/XRMServices/2011/Organization.svc", serverAddress,port));

                serverConfig.Credentials = new ClientCredentials();
                serverConfig.Credentials.UserName.UserName = string.Format("{0}@{1}", userName, userDomain);
                serverConfig.Credentials.UserName.Password = CrmServiceHelper.ConvertToUnsecureString(userPassword);
            }
            else if (authType == 1) //AD on-premise
            {

            }
            else if (authType == 2)//online office365
            {

            }
            else
                throw new Exception("Invalid AuthType");

            return serverConfig;
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            TabConnect.Visibility = Visibility.Visible;
        }
    }
}
