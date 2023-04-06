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
        private ViewModel model = null;
        private ChatRoomDB roomDB = new ChatRoomDB();

        public Private_Chate(string login, string sendLogin, bool isRequest)
        {
            InitializeComponent();

            this.DataContext = this;

            Login = login;

            SendLogin = sendLogin;
            IsRequest = isRequest;

            model = new ViewModel();

            point = IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == Login).IPEndPoint);

            //client = new TcpClient();

                //client.Connect(point);

                //ns = client.GetStream();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsRequest)
            {
                await ConnectAsync();
            }
        }

        public string Login { get; set; }
        public string SendLogin { get; set; }
        public bool IsRequest { get; set; }
        public string Conection { get; set; }

        private void SendConnect()
        {
            listener = new TcpListener(point);
            listener.Start(10);
            try
            {
                _privateMesseges.Add(new MessegeInfo("Waiting for conection"));

                client = listener.AcceptTcpClient();

                while (client.Connected)
                {
                    _privateMesseges.Add(new MessegeInfo("Connected!"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SendConnect   " + ex.Message);
            }

        }

        private Task ConnectAsync()
        {
            return Task.Run(() =>
            {
                listener = new TcpListener(point);

                try
                {
                    listener.Start();

                    Wait("Waiting for conection");

                    client = listener.AcceptTcpClient();

                    while (client.Connected)
                    {
                        Wait("Connected!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void Wait(string msg) 
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Conection = msg;
            }));

        }

        //private IPEndPoint GetIp(string login)
        //{
        //    return IPEndPoint.Parse(roomDB.Clients.FirstOrDefault(c => c.Login == Login).IPEndPoint);
        //}

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
           
            //try
            //{
            //    while (true)
            //    {
            //        string message = msgTB.Text;

            //        sw = new StreamWriter(ns);
            //        sw.WriteLine(message);

            //        sw.Flush();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //finally
            //{
            //    sw.Close();
            //    sr.Close();
            //    ns.Close();
            //    client.Close();
            //}
        }

        private void Listen()
        {
            while (true)
            {
                sr = new StreamReader(ns);
                string response = sr.ReadLine();

                _privateMesseges.Add(new MessegeInfo(response));
            }
        }
    }
}
