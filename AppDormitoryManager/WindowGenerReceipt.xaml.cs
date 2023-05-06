using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System.Data;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace AppDormitoryManager
{
    public partial class WindowGenerReceipt : Window
    {
        private int? role;
        private string fullNameUser;

        public WindowGenerReceipt(string fullNameUser, int? role)
        {
            InitializeComponent();

            // фильтр для выпадающего списка - не выводим ушедших жильцов
            using (var db = new dormitoryManagerBDEntities())
            {
                DateTime currentDate = DateTime.Now;
                TenantsComboBox.ItemsSource = db.Tenants
                    .Where(t => !string.IsNullOrEmpty(t.FullNameTenant) && t.DateEviction == null)
                    .ToList();
                TenantsComboBox.DisplayMemberPath = "FullNameTenant";
                TenantsComboBox.SelectedValuePath = "ID_Tenant";
            }
        }

        // Кнопка для генерации Word квитанций
        private void btnReceipt_Click(object sender, RoutedEventArgs e)
        {
            var selectedResidentId = (int)TenantsComboBox.SelectedValue;

            var wordApp = new Word.Application();
            wordApp.Visible = false;
            var wordDoc = wordApp.Documents.Add();

            using (var db = new dormitoryManagerBDEntities())
            {
                // Фиксируем выбранного жильца для дальнйшей работы
                var resident = db.Tenants.FirstOrDefault(r => r.ID_Tenant == selectedResidentId);
                if (resident == null)
                {
                    throw new Exception("Такого жильца нету в базе данных.");
                }

                // счётчик даты для более точного определения дня выплаты квитанции
                DateTime DataPay;
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = resident.DateChecin.Value.Day;
                if (day > DateTime.Now.Day)
                {
                    DataPay = new DateTime(year, month, day);
                }
                else
                {
                    if (month == 12)
                    {
                        year += 1;
                        month = 1;
                    }
                    else
                    {
                        month += 1;
                    }
                    DataPay = new DateTime(year, month, day);
                }

                var receipt = new Receipt()
                {
                    TenantReceipt = resident.ID_Tenant, // выбранный жилец из выпадающего списка
                    PayLiving = new Random().Next(1000, 5000), // имитация необходимой опалты за проживание
                    PayAddService = new Random().Next(1000, 3000), // имитация необходимой опалты за доп. услуги
                    DataPay = DataPay // дата выплаты квитанции
                };
                
                var paragraph = wordDoc.Paragraphs.Add();
                paragraph.Range.Text = "ФИО жильца: " + resident.FullNameTenant + "\n";
                paragraph.Range.Text += "Номер комнаты: " + resident.RoomTenant + "\n";
                paragraph.Range.Text += "Дата оплаты квитанции: " + receipt.DataPay.Value.ToShortDateString() + "\n";
                paragraph.Range.Text += "Сумма оплаты за проживание: " + receipt.PayLiving.ToString() + "\n";
                paragraph.Range.Text += "Сумма оплаты доп. услуг: " + receipt.PayAddService.ToString() + "\n";
                paragraph.Range.Text += "Итоговая сумма оплаты: " + (receipt.PayLiving + receipt.PayAddService).ToString() + "\n";
            }

            // Сохранение документа Word
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word documents|*.docx";
            if (saveFileDialog.ShowDialog() == true)
            {
                wordDoc.SaveAs2(saveFileDialog.FileName);
            }
            wordDoc.Close();
            wordApp.Quit();
        }


        private void btnBackMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Выводит текст выбранного жильца из выпадающего списка
        private void TenantsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTenant = TenantsComboBox.SelectedItem as Tenant;
            if (selectedTenant != null)
            {
                TenantInfoTextBox.Text = $"Выбран жилец: {selectedTenant.FullNameTenant}";
            }
        }
    }
}
