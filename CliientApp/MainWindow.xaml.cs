using Command_And_Members;
using MaterialDesignThemes.Wpf.Converters;
using PropertyChanged;
using System;
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
using static CliientApp.ViewModel;

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
        private string _privateChateLogin;

        public MainWindow()
        {
            InitializeComponent();

            model = new ViewModel();

            this.DataContext = model;

            string serverAdress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"])!;

            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAdress), serverPort);

            LeaveBtn.IsEnabled = false;

            JoinBtn.IsEnabled = false;

            SendBtn.IsEnabled = false;
        }

        private void Connect()
        {
            _client = new UdpClient();
        }
        private void Disconnect()
        {
            _client.Close();
        }

        private async void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = _login + " : " + msgTB.Text;
            SendMsg(msg);
        }

        private void JoinBtnClick(object sender, RoutedEventArgs e)
        {
            Connect();
            string msg = Commands.JOIN_CMD + _login;
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
                string login = new string(msg.Except(Commands.ADD_CMD).ToArray());

                model.AddMember(new MemberInfo(login));
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
            try
            {
                while (true)
                {
                    var result = await _client.ReceiveAsync();

                    string msg = Encoding.UTF8.GetString(result.Buffer);

                    if (msg.Contains(Commands.ADD_CMD))
                        AddNewMember(msg);
                    else if (msg.Contains(Commands.NEW_MSG_CMD))
                        NotifyAboutPrivateChat(msg);
                    else if (msg.Contains(Commands.OPEN_SENT_CHAT_CMD))
                        StartPrivateChate(msg);
                    else
                        model.Add(new MessegeInfo(msg));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NotifyAboutPrivateChat(string msg)
        {
            string login = new string(msg.Except(Commands.NEW_MSG_CMD).ToArray());

            model.NotifyAboutNewChat(login);
        }

        private void LeaveBtnClick(object sender, RoutedEventArgs e)
        {
            SendMsg(Commands.LEAVE_CMD);

            Disconnect();

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
                if (item.IsSelected)
                {
                    SendMsg(Commands.PRIVATE_CMD + item.Login);
                    _privateChateLogin = item.Login;
                    item.IsSelected = false;
                }
            }
        }

        private void StartPrivateChate(string msg)
        {
            string ip = new string(msg.Except(Commands.OPEN_SENT_CHAT_CMD).ToArray());

            Disconnect();

            Private_Chate chate = new Private_Chate(_login, _privateChateLogin, IPEndPoint.Parse(ip));

            chate.ShowDialog();
        }
    }

    public class ViewModel
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

        public void NotifyAboutNewChat(string login)
        {
            foreach (var m in _members)
            {
                if (m.Login == login)
                {
                    m.Post = "You have a new post";
                    m.IsRequest = true;
                }
            }
        }

        public MemberInfo GetMember(string login)
        {
            foreach (var m in _members)
            {
                if (m.Login == login)
                    return m;
            }

            return new MemberInfo();
        }
    }

    //[AddINotifyPropertyChangedInterface]
    //public class MemberInfo
    //{
    //    public string Login { get; set; }
    //    public string Post { get; set; }
    //    public bool IsSelected { get; set; }
    //    public bool IsRequest { get; set; }
    //    public string Initial { get; }

    //    public MemberInfo() { }

    //    public MemberInfo(string login)
    //    {
    //        Login = login;

    //        Initial = Login.ToCharArray().First().ToString();

    //        IsRequest = false;
    //    }
    //}
}