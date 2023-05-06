using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AppDormitoryManager
{
    public partial class MainWindow : Window
    {
        private dormitoryManagerBDEntities _db = new dormitoryManagerBDEntities();
        private int _accessCode;
        private bool _isCodeValid = true;

        public MainWindow()
        {
            InitializeComponent();
            // Блокируем доступ ко всем элемнтам окна, кроме строки Логина и кнопки "Сброс"
            LoginButton.IsEnabled = false;
            txtPassword.IsEnabled = false;
            tbAccessCode.IsEnabled = false;
            btnSMSgen.IsEnabled = false;
        }

        // Основа аторизации
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            VerifyUser();
        }

        //Кнопка броса данных в окне
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtLogin.Clear();
            txtPassword.Clear();
            tbAccessCode.Clear();
            LoginButton.IsEnabled = false;
            txtPassword.IsEnabled = false;
            tbAccessCode.IsEnabled = false;
            btnSMSgen.IsEnabled = false;
        }

        // Проверка логина
        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var query = from login in _db.Logins
                            where login.LoginName == txtLogin.Text
                            select login;

                var userLogin = query.FirstOrDefault();

                if (userLogin != null)
                {
                    txtPassword.IsEnabled = true;
                    txtPassword.Focus();
                }
                else
                {
                    MessageBox.Show("Логин не найден.");
                }
            }
        }

        // Проверка пароля
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                VerifyUser();
            }
        }

        // Часть авторизации и сохранение необходимых данных
        private async void VerifyUser()
        {
            var query = from user in _db.Users
                        join login in _db.Logins on user.LoginUser equals login.ID_Login
                        where login.LoginName == txtLogin.Text && login.Password == txtPassword.Password
                        select new { user.FullNameUser, login.Role };

            var userInfo = query.FirstOrDefault();

            if (userInfo != null)
            {
                GenerateAccessCode();
                tbAccessCode.IsEnabled = true;
                tbAccessCode.Focus();
                btnSMSgen.IsEnabled = true;
                LoginButton.IsEnabled = true;

                await Task.Delay(10000);
                _isCodeValid = false;
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        // Проверка кода доступа и открытие доступа к функционалу системы
        private void AccessCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_isCodeValid && tbAccessCode.Text == _accessCode.ToString())
                {
                    var query = from user in _db.Users
                                join login in _db.Logins on user.LoginUser equals login.ID_Login
                                where login.LoginName == txtLogin.Text && login.Password == txtPassword.Password
                                select new { user.FullNameUser, login.Role };

                    var userInfo = query.FirstOrDefault();

                    string role = userInfo.Role == 1 ? "Администратор" : "Комендант";
                    MessageBox.Show($"Добро пожаловать, {userInfo.FullNameUser}! Ваша роль: {role}.");
                    WindowTenants tenantsWindow = new WindowTenants(userInfo.FullNameUser, userInfo.Role);
                    WindowGenerReceipt grWindow = new WindowGenerReceipt(userInfo.FullNameUser, userInfo.Role);
                    WindowGeneral generalWindow = new WindowGeneral(userInfo.FullNameUser, userInfo.Role);
                    generalWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный код доступа.");
                }
            }
        }
        
        private void btnSMScode_Click(object sender, RoutedEventArgs e)
        {
            GenerateAccessCode();
        }

        // Генерация рандомного кода из 4 цифр
        private async void GenerateAccessCode()
        {
            _accessCode = new Random().Next(1000, 10000);
            MessageBox.Show($"Код доступа: {_accessCode}");
            _isCodeValid = true;

            await Task.Delay(10000);
            _isCodeValid = false;
        }
    }
}
