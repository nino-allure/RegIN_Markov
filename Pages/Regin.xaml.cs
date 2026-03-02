using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace RegIN_Markov.Pages
{
    /// <summary>
    /// Логика взаимодействия для Regin.xaml
    /// </summary>
    public partial class Regin : Page
    {
        public Regin()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogin.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogin.HandlerInCorrectLogin += InCorrectLogin;
            FileDialogImage.Filter = "PNG (* png)|* .png|JPG (* .jpg)|* .jpg";
            FileDialogImage.RestoreDirectory = true;
            FileDialogImage.Title = "Choose a photo for you pfp";

        }
        OpenFileDialog FileDialogImage = new OpenFileDialog();
        bool BCorrectLogin = false;
        bool BCorrectPassword = false;
        bool BCorrectConfirmPassword = false;
        bool BSetImages = false;

        private void CorrectLogin()
        {
            SetNotification("Login already in use", Brushes.Red);
            BCorrectLogin = false;
        }
        private void InCorrectLogin() =>
            SetNotification("", Brushes.Black);
        private void SetLogin(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
            {
                // Вызываем метод ввода логина
                SetLogin();
            }
        }

        /// <summary>
        /// Метод ввода логина
        /// </summary>
        private void SetLogin(object sender, System.Windows.RoutedEventArgs e) =>
            // Вызываем метод ввода логина
            SetLogin();

        /// <summary>
        /// Метод ввода логина
        /// </summary>
        public void SetLogin()
        {
            // Регулярное выражение для почты
            Regex regex = new Regex(@"^[a-zA-Z0-9._-]{4,}@[a-zA-Z0-9._-]{2,}\.[a-zA-Z0-9._-]{2,}$");

            // Введён ли логин зависит от того регулярного выражения
            BCorrectLogin = regex.IsMatch(TbLogin.Text);

            // Если регулярное выражение совпадает
            if (regex.IsMatch(TbLogin.Text) == true)
            {
                // Выводим пустое уведомление чёрным цветом
                SetNotification("", Brushes.Black);

                // Вызываем получение данных пользователя по логину
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
            }
            else
            {
                // Если введён логин не удовлетворяющий регулярное выражение, выводим сообщение красным цветом
                SetNotification("Invalid login", Brushes.Red);
            }

            // Вызываем метод авторизации
            OnRegin();
        }
        #region SetPassword

        /// <summary>
        /// Метод ввода пароля
        /// </summary>
        private void SetPassword(object sender, System.Windows.RoutedEventArgs e) =>
            // Вызываем метод ввода пароля
            SetPassword();

        /// <summary>
        /// Метод ввода пароля
        /// </summary>
        private void SetPassword(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем метод ввода пароля
                SetPassword();
        }

        /// <summary>
        /// Метод ввода пароля
        /// </summary>
        public void SetPassword()
        {
            // Регулярное выражение для проверки сложности пароля
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[!@#$%&?*\-_=])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%&?*\-_=]{10,}");

            // Пояснения к регулярному выражению:
            // (?=.*[0-9]) - строка содержит хотя бы одно число;
            // (?=.*[!@#$%&?*\-_=]) - строка содержит хотя бы один спецсимвол;
            // (?=.*[a-z]) - строка содержит хотя бы одну латинскую букву в нижнем регистре;
            // (?=.*[A-Z]) - строка содержит хотя бы одну латинскую букву в верхнем регистре;
            // [0-9a-zA-Z!@#$%&?*\-_=]{10,} - строка состоит не менее, чем из 10 вышеупомянутых символов.

            // Введён ли пароль зависит от результата регулярного выражения
            BCorrectPassword = regex.IsMatch(TbPassword.Password);

            // Если введённый пароль удовлетворяет регулярное выражение
            if (regex.IsMatch(TbPassword.Password) == true)
            {
                // Выводим пустое сообщение с чёрным цветом
                SetNotification("", Brushes.Black);

                // Если пароль уже введён для повторения
                if (TbConfirmPassword.Password.Length > 0)
                    // Вызываем проверку повторения пароля
                    ConfirmPassword(true);

                // Вызываем функцию регистрации
                OnRegion();
            }
            else
            {
                // Если введённый пароль не удовлетворяет регулярное выражение
                // Выводим сообщение с ошибкой красным цветом
                SetNotification("Invalid password", Brushes.Red);
            }
        }
        #endregion
    }
}
