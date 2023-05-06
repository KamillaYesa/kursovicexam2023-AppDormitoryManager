using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AppDormitoryManager
{
    public partial class TenantsComendant : Page
    {
        public TenantsComendant()
        {
            InitializeComponent();
            Loaded += TenantsComendant_Loaded;
        }

        // Выводит информацию об жильцах из базы данных
        private void TenantsComendant_Loaded(object sender, RoutedEventArgs e)
        {
            using (var context = new dormitoryManagerBDEntities())
            {
                var tenants = context.Tenants.ToList();

                TenantsList.ItemsSource = tenants;
            }
        }

        // Кнопка возврата в Главное меню пользователя
        private void btnBackMenu_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}

