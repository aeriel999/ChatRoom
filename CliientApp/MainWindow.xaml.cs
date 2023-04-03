using MaterialDesignThemes.Wpf.Converters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
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
        private ViewModel model = null;
        private IPEndPoint _serverEndPoint;
        private UdpClient _client;
        private string _login;
        private string _adding = "<ADD.MEMBER>";
        public MainWindow()
        {
            InitializeComponent();

            model = new ViewModel();
            this.DataContext = model;

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
            string msg = "$<join>" + _login;
            SendMsg(msg);
            Listen();

            LeaveBtn.IsEnabled = true;
            JoinBtn.IsEnabled = false;
            SendBtn.IsEnabled = true;
        }

        private void AddNewMember(string msg)
        {
            try
            {
                string[] new_member = msg.Split(new char[] { '$' });

                model.AddMember(new MemberInfo(new_member[1]));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                var result = await _client.ReceiveAsync();

                string msg = Encoding.UTF8.GetString(result.Buffer);

                if (msg.Contains(_adding))
                    AddNewMember(msg);
                else
                    model.Add(new MessegeInfo(msg));
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
                LoginBtn.IsEnabled = false;
                _login = loginForm.Login!;
               // LoginLB.Content = _login;
            }
        }

        private void OpenPrivateChateBtnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in model.Members)
            {
                if(item.IsSelected)
                    MessageBox.Show(item.Login);

            }

            //Private_Chate chate = new Private_Chate();

            //chate.ShowDialog();
        }
    }

    class ViewModel
    {
        private ObservableCollection<MessegeInfo> _messeges = new ObservableCollection<MessegeInfo>();
        private ObservableCollection<MemberInfo> _members = new ObservableCollection<MemberInfo>();

        public IEnumerable<MessegeInfo> Messeges => _messeges;
        public void Add(MessegeInfo info)
        {
            _messeges.Add(info);
        }

        public IEnumerable<MemberInfo> Members => _members;
        public void AddMember(MemberInfo info)
        {
            _members.Add(info);
        }

        public ViewModel()
        {
            AddMember(new MemberInfo("Login"));
        }
    }

    
    class MemberInfo
    {
        public string Login { get; set; }

        public bool IsSelected { get; set; }

        public string Initial { get; set; } = "L";

        public MemberInfo(string login)
        {
            Login = login;
        }
    }
}
