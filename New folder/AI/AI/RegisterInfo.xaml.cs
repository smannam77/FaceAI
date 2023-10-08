using System.Windows;

namespace AI
{
    /// <summary>
    /// Interaction logic for RegisterInfo.xaml
    /// </summary>
    public partial class RegisterInfo : Window
    {
        public RegisterInfo()
        {
            InitializeComponent();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();
            this.Close();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
