using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AppDormitoryManager
{
    public partial class TenantsAdmin : Page
    {
        public TenantsAdmin()
        {
            InitializeComponent();
            Loaded += TenantsComendant_Loaded;
        }

        private void TenantsComendant_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Собирает информацию об жильцах из базы данных
        private void LoadData()
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

        // Кнопка добавления жильца
        private void btnSaveTenant_Click(object sender, RoutedEventArgs e)
        {
            var addTenantWindow = new AddTenant();
            addTenantWindow.ShowDialog();
            LoadData();
        }

        // Кнопка изменения данных об жильце
        private void btnChangeTenant_Click(object sender, RoutedEventArgs e)
        {
            if (TenantsList.SelectedItem is Tenant selectedTenant)
            {
                var editTenantWindow = new AddTenant(selectedTenant);
                editTenantWindow.ShowDialog();
                LoadData();
            }
        }

        // Кнопка удаления жильца из базы данных
        private void btnDeleyeTenant_Click(object sender, RoutedEventArgs e)
        {
            if (TenantsList.SelectedItem is Tenant selectedTenant)
            {
                if (MessageBox.Show($"Вы уверены, что хотите удалить жильца {selectedTenant.FullNameTenant}?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new dormitoryManagerBDEntities())
                    {
                        var existingTenant = db.Tenants.FirstOrDefault(t => t.ID_Tenant == selectedTenant.ID_Tenant);
                        if (existingTenant != null)
                        {
                            db.Tenants.Remove(existingTenant);
                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
            }
        }
    }
}
