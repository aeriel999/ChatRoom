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
            if (isRegistre.IsChecked == true)
                Registre();

            if (roomDB.Clients.Where(c => c.Login == NameTextBox.Text)
                .Where(c => c.Password == PasswordBox.Password).Select(a => a.Login).FirstOrDefault() != null)
            {
                IsLogin = true;

                this.Close();
            }
            else
            {
                MessageBox.Show("Login Or Password not correct");
            }
        }

        private void Registre()
        {
            if (NameTextBox.Text.Length < 4 || PasswordBox.Password.Length < 4)
            {
                NameTextBox.Text = string.Empty;
                PasswordBox.Password = string.Empty;

                MessageBox.Show("Login or password must be at least 3 symbols");
            }
            else 
            {
                try
                {
                    roomDB.Clients.Add(new Client() { Login = NameTextBox.Text, Password = PasswordBox.Password });
                    roomDB.SaveChanges();
                    IsLogin = true;

                    MessageBox.Show("Succsses!");

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
