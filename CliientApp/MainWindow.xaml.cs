using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CliientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint serverEndPoint;
        UdpClient client;

        ObservableCollection<MessegeInfo> messeges = new ObservableCollection<MessegeInfo>();
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = messeges;

            client = new UdpClient();

            string serverAdress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"])!;

            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAdress), serverPort);

            LeaveBtn.IsEnabled = false;

            JoinBtn.IsEnabled = false;

            SendBtn.IsEnabled = false;
        }

        private async void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = msgTB.Text;
            SendMsg(msg);
        }

        private void JoinBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = "$<join>";
            SendMsg(msg);
            Listen();
        }

        private async void SendMsg(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            await client.SendAsync(data, data.Length, serverEndPoint);
        }

        private async void Listen()
        {
            while (true)
            {
                IPEndPoint? remoteIp = null;
                var result = await client.ReceiveAsync();

                string msg = Encoding.UTF8.GetString(result.Buffer);

                messeges.Add(new MessegeInfo(msg));
            }
        }

        private void LeaveBtnClick(object sender, RoutedEventArgs e)
        {
            SendMsg("$<leave>");

            client.Close();
        }

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();

            loginForm.ShowDialog();
        }
    }
}
