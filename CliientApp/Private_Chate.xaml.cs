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
        private IPEndPoint point = null;
        private TcpClient client = null;
        private TcpListener listener = null;
        private NetworkStream ns = null;
        private StreamWriter sw = null;
        private StreamReader sr = null;
        private ObservableCollection<MessegeInfo> _privateMesseges = new ObservableCollection<MessegeInfo>();
        private ChatRoomDB roomDB = new ChatRoomDB();

        public Private_Chate(string login, string sendLogin, bool isRequest)
        {
            InitializeComponent();

            this.DataContext = this;

            Login = login;

            SendLogin = sendLogin;

            IsRequest = isRequest;

            Conection = "Waiting for conection...";
        }
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
            point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == Login).IPEndPoint);

            return Task.Run(() =>
            {
                listener = new TcpListener(point);

                try
                {
                    listener.Start();

                    client = listener.AcceptTcpClient();

                    //Conection = "Connected!";

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });
        }

        private Task GetConnectAsync()
        {
            point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == SendLogin).IPEndPoint);

            client = new TcpClient();

            return Task.Run(() =>
            {
                try
                {
                    client.ConnectAsync(point);

                    //Conection = "Connected!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        public string Login { get; set; }
        public string SendLogin { get; set; }
        public bool IsRequest { get; set; }
        public string Conection { get; set; }
        public IEnumerable<MessegeInfo> Messeges => _privateMesseges;

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = msgTB.Text;
                _privateMesseges.Add(new MessegeInfo($"You: {message}"));

                sw = new StreamWriter(ns);
                sw.WriteLine(message);

                sw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //finally
            //{
            //    sw.Close();
            //    sr.Close();
            //    //ns.Close();
            //    client.Close();
            //}
        }

        private async void Listen()
        {
            Conection = "Connected!";

            try
            {
                while (client.Connected)
                {
                    ns = client.GetStream();

                    sr = new StreamReader(ns);

                    string response = await sr.ReadLineAsync();

                    _privateMesseges.Add(new MessegeInfo($"{SendLogin}: {response}"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendConnect()
        {
            point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == Login).IPEndPoint);

            listener = new TcpListener(point);

            try
            {
                listener.Start();

                client = listener.AcceptTcpClient();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetConnect()
        {
            point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == SendLogin).IPEndPoint);

            client = new TcpClient();

            try
            {
                client.ConnectAsync(point);

                Listen();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
            private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRequest)
            {
                    SendConnect();
            }
            else
                GetConnect();
        }
    }
}
