using Command_And_Members;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
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
        private bool _isRquest;
        private bool _leavePublicChat;

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

            _isRquest = false;
        }

        private async void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string msg = _login + " : " + msgTB.Text;
            SendMsg(msg);
            msgTB.Text = string.Empty;
        }

        private void JoinBtnClick(object sender, RoutedEventArgs e)
        {
            _client = new UdpClient();

            SendMsg(Commands.JOIN_CMD + _login);

            _leavePublicChat = false;

            Listen();

            LeaveBtn.IsEnabled = true;
            JoinBtn.IsEnabled = false;
            SendBtn.IsEnabled = true;
        }

        private void AddNewMember(string msg)
        {
            try
            {
                string login = msg.Substring(Commands.JOIN_CMD.Length);

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
                while (!_leavePublicChat)
                {
                    var result = await _client.ReceiveAsync();

                    string msg = Encoding.UTF8.GetString(result.Buffer);

                    if (msg.Contains(Commands.JOIN_CMD))
                        AddNewMember(msg);
                    else if (msg.Contains(Commands.PRIVATE_CMD))
                        NotifyAboutPrivateChat(msg);
                    else if (msg.Contains(Commands.LEAVE_CMD))
                        DeleteMemberFromChat(msg);
                    else if (msg.Contains(Commands.EXIST_CMD))
                        IfUserIsExist();
                    else
                        model.AddMsg(new MessegeInfo(msg));
                }
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void IfUserIsExist()
        {
            model.AddMsg(new MessegeInfo("You are already sign in in another device!"));

            loginTB.Text = string.Empty;

            LeaveBtn.IsEnabled = false;

            JoinBtn.IsEnabled = false;

            SendBtn.IsEnabled = false;

            LoginBtn.IsEnabled = true;
        }

        private void DeleteMemberFromChat(string msg)
        {
            try
            {
                string login = new string(msg.Except(Commands.LEAVE_CMD).ToArray());

                model.DeleteMember(login);
            }
            catch (Exception)
            {
                MessageBox.Show("One of the members leave chat");
            }
        }

        private void NotifyAboutPrivateChat(string msg)
        {
            string login = msg.Substring(Commands.PRIVATE_CMD.Length);

            model.NotifyAboutNewChat(login);

            _isRquest = true;
        }

        private void LeaveBtnClick(object sender, RoutedEventArgs e)
        {
            SendMsg(Commands.LEAVE_CMD + _login);

            _leavePublicChat = true;

            model.DeleteAllMembers();

            JoinBtn.IsEnabled = true;
            LeaveBtn.IsEnabled = false;
            SendBtn.IsEnabled = false;
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
                loginTB.Text = _login;
            }
        }

        private void OpenPrivateChateBtnClick(object sender, RoutedEventArgs e)
        {
            _leavePublicChat = true;

            foreach (var item in model.Members)
            {
                if (item.IsSelected)
                {
                    if (!_isRquest)
                    {
                        SendMsg(Commands.PRIVATE_CMD + item.Login);
                    }

                    _privateChateLogin = item.Login;

                    item.IsSelected = false;

                    item.Post = "";
                }
            }

            Private_Chate chate = new Private_Chate(_login, _privateChateLogin, _isRquest);

            chate.ShowDialog();

            _leavePublicChat = false;

            _isRquest = false;

            Listen();
        }

        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.Close();
            }
            catch (Exception) { }
            this.Close();
        }
    }
}