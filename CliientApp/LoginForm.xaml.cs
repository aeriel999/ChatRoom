using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CliientApp
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        private ChatRoomDB roomDB = new ChatRoomDB();
        public bool IsLogin { get; set; }
        public LoginForm()
        {
            InitializeComponent();

            IsLogin = false;
        }

        private void GoBtnClick(object sender, RoutedEventArgs e)
        {

            if (roomDB.Clients.Where(c => c.Login == NameTextBox.Text).Where(c => c.Password == PasswordBox.Password).Select(a => a.Login).FirstOrDefault() != null)
            {
                IsLogin = true;

                MessageBox.Show("Ok");
            }
            else
            {
                MessageBox.Show("False");

            }
            this.Close();
        }
    }
}
