using System.Windows;

namespace AppDormitoryManager
{
    public partial class WindowTenants : Window
    {
        private int? role;

        //в зависимоти от роли октрываем соответсвующий фрейм
        public WindowTenants(string fullNameUser, int? role)
        {
            this.role = role;

            InitializeComponent();
            if (role == 1)
            {
                MainFrame.Navigate(new TenantsAdmin());
            }
            else if (role == 2)
            {
                MainFrame.Navigate(new TenantsComendant());
            }
        }
    }
}
