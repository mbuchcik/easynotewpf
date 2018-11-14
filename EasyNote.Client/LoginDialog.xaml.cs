using EasyNote.Integration.EasyNoteAPI.Model;
using System.Windows;

namespace EasyNote.Client
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Globals.Credentials = new UserInfo
            {
                Email = txtName.Text,
                Password = txtPassword.Password
            };
            this.DialogResult = true;
        }
    }
}
