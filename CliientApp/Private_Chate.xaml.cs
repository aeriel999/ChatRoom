using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using Command_And_Members;
using Azure;
using Azure.Core;
using System.Diagnostics;
using System.Windows.Interop;

namespace CliientApp
{
    /// <summary>
    /// Interaction logic for Private_Chate.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class Private_Chate : Window
    {
        private TcpClient client = null; 
        private ObservableCollection<MessegeInfo> _privateMesseges = new ObservableCollection<MessegeInfo>();
        private ChatRoomDbContext roomDB = new ChatRoomDbContext(ConfigurationManager.ConnectionStrings["ChatRoomDb"].ConnectionString);

        public Private_Chate(string login, string sendLogin, bool isRequest)
        {
            InitializeComponent();

            this.DataContext = this;

            Login = login;

            SendLogin = sendLogin;

            IsRequest = isRequest;

            Conection = "Waiting for conection...";

        }
        public string Login { get; set; }
        public string SendLogin { get; set; }
        public bool IsRequest { get; set; }
        public string Conection { get; set; }
        public IEnumerable<MessegeInfo> Messeges => _privateMesseges;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsRequest)
                await SendConnectAsync();
            else
                await GetConnectAsync();

            Listen();
        }

        private Task SendConnectAsync()
        {
            var point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == Login).IPEndPoint);

            return Task.Run(() =>
            {
               var listener = new TcpListener(point);

                try
                {
                    listener.Start();

                    client = listener.AcceptTcpClient();

                    listener.Stop();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(Login + "   SendConnectAsync   " + ex.Message);
                }

            });
        }

        private Task GetConnectAsync()
        {
            var point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == SendLogin).IPEndPoint);

             client = new TcpClient();

                return Task.Run(() =>
                {
                    try
                    {
                        client.ConnectAsync(point);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Login + "   GetConnectAsync   " + ex.Message);
                    }
                });
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = msgTB.Text;
                _privateMesseges.Add(new MessegeInfo($"You: {message}"));

               var ns = client.GetStream();

                using var sw = new StreamWriter(ns, leaveOpen:true);

                sw.WriteLine(message);

                sw.Flush();

                msgTB.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Login + "   SendBtn_Click   " + ex.Message);
            }
        }

        private async void Listen()
        {
            Conection = "Connected!";

            try
            {
                while (client.Connected)
                {
                    var ns = client.GetStream();


                    using var sr = new StreamReader(ns, leaveOpen: true);

                    var response = await sr.ReadLineAsync();

                    _privateMesseges.Add(new MessegeInfo($"{SendLogin}: {response}"));
                }
            }
            catch (IOException) { }
            catch (Exception ex)
            {
                MessageBox.Show(Login + "   Listen  " + ex.Message);
            }
        }

        private void ExitBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            roomDB.Dispose();
           //client?.Close();
        }
    }
}
