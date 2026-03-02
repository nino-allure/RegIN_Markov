using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegIN_Markov.Pages
{
    /// <summary>
    /// Логика взаимодействия для Recovery.xaml
    /// </summary>
    public partial class Recovery : Page
    {
        public Recovery()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogin.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogin.HandlerInCorrectLogin += InCorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        string OldLogin;
        bool isCapture = false;
        private void CorrectLogin()
        {
            if (OldLogin != TbLogin.Text)
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogin.Name, Brushes.Black);
                try
                {
                    BitmapImage biImg = new BitmapImage();
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogin.Image);
                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();

                    ImageSource imgSrc = biImg;

                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                    StartAnimation.Completed += delegate
                    {
                        User.Source = imgSrc;

                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                        User.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    User.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

                OldLogin = TbLogin.Text;
                SendNewPassword();
            }
        }
        private void InCorrectLogin()
        {
            // Если пользователь идентифицирован как личность, или указаны ошибки
            if (LNameUser.Content != "")
            {
                // Очищаем приветствие пользователя
                LNameUser.Content = "";

                // Создаём анимацию старта
                DoubleAnimation StartAnimation = new DoubleAnimation();
                // Указываем значение от которого она выполняется
                StartAnimation.From = 1;
                // Указываем значение до которого она выполняется
                StartAnimation.To = 0;
                // Указываем продолжительность выполнения
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                // Присваиваем событие при конце анимации
                StartAnimation.Completed += delegate
                {
                    // Указываем стандартный логотип в качестве изображения пользователя
                    User.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-user.png"));

                    // Создаём анимацию конца
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    EndAnimation.From = 0;
                    // Указываем значение до которого она выполняется
                    EndAnimation.To = 1;
                    // Указываем продолжительность выполнения
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                    // Запускаем анимацию плавной смены на изображении
                    User.BeginAnimation(Image.OpacityProperty, EndAnimation);
                };

                // Запускаем анимацию плавной смены на изображении
                User.BeginAnimation(Image.OpacityProperty, StartAnimation);
            }

            // Сообщение о том что логин введён неправильно
            if (TbLogin.Text.Length > 0)
                // Выводим сообщение о том, что логин введён не верно, цвет текста красный
                SetNotification("Login is incorrect", Brushes.Red);
        }
        private void CorrectCapture()
        {
            Capture.IsEnabled = false;
            isCapture = true;
            SendNewPassword();
        }
        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text);
        }
        private void SetLogin(object sender, RoutedEventArgs e) =>
            MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text);

        public void SendNewPassword()
        {
            // Если пройдена капча
            if (isCapture)
            {
                // Если пароль не является пустым, а это значит пользователь ввёл правильную почту
                if (MainWindow.mainWindow.UserLogin.Password != String.Empty)
                {
                    // Создаём анимацию старта
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    StartAnimation.From = 1;
                    // Указываем значение до которого она выполняется
                    StartAnimation.To = 0;
                    // Указываем продолжительность выполнения
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                    // Присваиваем событие при конце анимации
                    StartAnimation.Completed += delegate
                    {
                        // Указываем стандартный логотип в качестве изображения пользователя
                        User.Source = new BitmapImage(new Uri("pack://application:,,,/Images/mail.jpg"));

                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                        // Запускаем анимацию плавной смены на изображении
                        User.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    // Запускаем анимацию плавной смены на изображении
                    User.BeginAnimation(Image.OpacityProperty, StartAnimation);

                    // Выводим сообщение о том что новый пароль будет отправлен на почту
                    SetNotification("An email has been sent to your email.", Brushes.Black);

                    // Вызываем функцию создания нового пароля
                    MainWindow.mainWindow.UserLogin.CreateNewPassword();
                }
            }
        }
        private void OpenLogin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}
