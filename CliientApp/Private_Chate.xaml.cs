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
        public string Login { get; set; }

        private IPEndPoint point = null;
        private TcpClient client = null;
        NetworkStream ns = null;
        StreamWriter sw = null;
        StreamReader sr = null;

        private ObservableCollection<MessegeInfo> messeges = new ObservableCollection<MessegeInfo>();
        public Private_Chate(string login, IPEndPoint endPoint)
        {
            InitializeComponent();

            this.DataContext = messeges;

           // Login = login;

           //point = endPoint;

           //client = new TcpClient();

           //client.Connect(point);

           //ns = client.GetStream();

           //Listen();
        }

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

                messeges.Add(new MessegeInfo(response));
            }
        }
    }
}
