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
        private NetworkStream ns = null;
        private StreamWriter sw = null;
        private StreamReader sr = null;
        private ObservableCollection<MessegeInfo> _privateMesseges = new ObservableCollection<MessegeInfo>();
        private ViewModel model = null;

        public Private_Chate(string sendLogin, string login, IPEndPoint endPoint)
        {
            InitializeComponent();

            this.DataContext = _privateMesseges;

            Login = login;

            SendLogin = sendLogin;

            model = new ViewModel();

           //point = endPoint;

            //client = new TcpClient();

            //client.Connect(point);

            //ns = client.GetStream();

            //Listen();
        }

        public string Login { get; set; }
        public string SendLogin { get; set; }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while (true)
                {
                    string message = msgTB.Text;

                    sw = new StreamWriter(ns);
                    sw.WriteLine(message);

                    sw.Flush(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Close();
                sr.Close();
                ns.Close();
                client.Close();
            }
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
