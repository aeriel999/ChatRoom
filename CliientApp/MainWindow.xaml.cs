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
        private IPEndPoint _serverEndPoint;
        private UdpClient _client;
        private ObservableCollection<MessegeInfo> _messeges = new ObservableCollection<MessegeInfo>();
        private string _login;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _messeges;

            _client = new UdpClient();

            string serverAdress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"])!;

            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAdress), serverPort);

            LeaveBtn.IsEnabled = false;

            JoinBtn.IsEnabled = false;

            SendBtn.IsEnabled = false;
        }

        private async void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = _login + " : " + msgTB.Text;
            SendMsg(msg);
        }

        private void JoinBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = "$<join>";
            SendMsg(msg);
            Listen();

            LeaveBtn.IsEnabled = true;
            JoinBtn.IsEnabled = false;
            SendBtn.IsEnabled = true;
        }

        private async void SendMsg(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            await _client.SendAsync(data, data.Length, _serverEndPoint);
        }

        private async void Listen()
        {
            while (true)
            {
                IPEndPoint? remoteIp = null;
                var result = await _client.ReceiveAsync();

                string msg = Encoding.UTF8.GetString(result.Buffer);

                _messeges.Add(new MessegeInfo(msg));
            }
        }

        private void LeaveBtnClick(object sender, RoutedEventArgs e)
        {
            SendMsg("$<leave>");

            _client.Close();

            JoinBtn.IsEnabled = true;
        }

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();

            loginForm.ShowDialog();

            if (loginForm.IsLogin == true)
            {
                JoinBtn.IsEnabled = true;

                _login = loginForm.Login!;
            }
        }
    }
}
