using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
        private TcpClient _tcpClient;
        private NetworkStream _ns;
        private StreamReader reader = null;
        private StreamWriter writer = null;
        private ObservableCollection<MessegeInfo> messeges = new ObservableCollection<MessegeInfo>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = messeges;

            _tcpClient = new TcpClient();

            string serverAdress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"])!;

            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAdress), serverPort);

            _ns = null;
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            writer.WriteLine(msgTB.Text);

            writer.Flush();
        }

        private void ConnectionBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tcpClient.Connect(_serverEndPoint);

                _ns = _tcpClient.GetStream();

                reader = new StreamReader(_ns);
                writer = new StreamWriter(_ns);

                Listen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void Listen()
        {
            while (true)
            {
                string? msg =  await reader.ReadLineAsync();
                
                messeges.Add(new MessegeInfo(msg));
            }
        }

        private void DisconnectBtnClick(object sender, RoutedEventArgs e)
        {
            writer.Close();
            reader.Close();
            _ns.Close();
            _tcpClient.Close();
        }
    }

    public class MessegeInfo
    {
        public string Messege { get; set; }
        public DateTime Time { get; set; }

        public MessegeInfo(string? text)
        {
            Messege = text ?? string.Empty;
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Messege} : {Time.ToShortTimeString()}";
        }
    }
}
