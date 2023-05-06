using System.Linq;
using System.Windows;

namespace AppDormitoryManager
{
    public partial class AddTenant : Window
    {
        public Tenant Tenant { get; set; }

        public AddTenant()
        {
            InitializeComponent();
        }

        public AddTenant(Tenant tenant) : this()
        {
            // Собираем данные с элемнтов этоого окна
            Tenant = tenant;
            FullNameTenantTextBox.Text = tenant.FullNameTenant;
            RoomTextBox.Text = tenant.RoomTenant.ToString();
            PhoneTextBox.Text = tenant.Phone.ToString();
            EmailTextBox.Text = tenant.Email;
            DateCheckinDatePicker.SelectedDate = tenant.DateChecin;
            DateEvictionDatePicker.SelectedDate = tenant.DateEviction;
        }

        // Создаёт или сохраняет изменния о данных жильца в базе данных
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность записи номера комнаты
            int room;
            if (!int.TryParse(RoomTextBox.Text, out room))
            {
                MessageBox.Show("Неверный номер комнаты");
                return;
            }

            // Проверка на правильность записи номера телефона
            int phone;
            if (!int.TryParse(PhoneTextBox.Text, out phone))
            {
                MessageBox.Show("Неверный номер телефона");
                return;
            }

            using (var db = new dormitoryManagerBDEntities())
            {

                // Если добавляем жильца в базу данных
                if (Tenant == null)
                {
                    var newTenant = new Tenant
                    {
                        FullNameTenant = FullNameTenantTextBox.Text,
                        RoomTenant = room,
                        Phone = phone,
                        Email = EmailTextBox.Text,
                        DateChecin = DateCheckinDatePicker.SelectedDate,
                        DateEviction = DateEvictionDatePicker.SelectedDate
                    };
                    var maxId = db.Tenants.Max(t => t.ID_Tenant);
                    newTenant.ID_Tenant = maxId + 1;
                    db.Tenants.Add(newTenant);
                }

                // Если изменяем данные жильца в базе данных
                else
                {
                    var existingTenant = db.Tenants.FirstOrDefault(t => t.ID_Tenant == Tenant.ID_Tenant);
                    if (existingTenant != null)
                    {
                        existingTenant.FullNameTenant = FullNameTenantTextBox.Text;
                        existingTenant.RoomTenant = room;
                        existingTenant.Phone = phone;
                        existingTenant.Email = EmailTextBox.Text;
                        existingTenant.DateChecin = DateCheckinDatePicker.SelectedDate;
                        existingTenant.DateEviction = DateEvictionDatePicker.SelectedDate;
                    }
                }
                db.SaveChanges();
            }
            this.Close();
        }

        // Кнопка закрытия этого окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
