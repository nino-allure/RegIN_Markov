using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
using RegIN_Markov.Classes;
using RegIN_Markov.Elements;

namespace RegIN_Markov.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogin.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogin.HandlerInCorrectLogin += InCorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        string OldLogin;
        int CountSetPassword = 2;
        bool IsCapture = false;
        public void CorrectLogin()
        {
            // Если старый логин не соответствует логину введённому в поле
            if (OldLogin != TbLogin.Text)
            {
                // Вызываем метод уведомления, передавая сообщение, имя пользователя и цвет
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);

                // Используем конструкцию try-catch
                try
                {
                    // Инициализируем BitmapImage, который будет содержать изображение пользователя
                    BitmapImage biImg = new BitmapImage();

                    // Открываем поток, хранилищем которого является память и указываем в качестве источника
                    // массив байт изображения пользователя
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);

                    // Сигнализируем о начале инициализации
                    biImg.BeginInit();
                    // Указываем источник потока
                    biImg.StreamSource = ms;
                    // Сигнализируем о конце инициализации
                    biImg.EndInit();

                    // Получаем ImageSource
                    ImageSource imgSrc = biImg;

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
                        // Устанавливаем изображение
                        IUser.Source = imgSrc;

                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    // Если возникла ошибка, выводим в дебаг
                    Debug.WriteLine(exp.Message);
                }

                // Запоминаем введённый логин
                OldLogin = TbLogin.Text;
        }
            }
        public void InCorrectLogin()
        {
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
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-user.png"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                };

                // Запускаем анимацию плавной смены на изображении
                IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
            }
            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);
        }
            
        public void CorrectCapture()
        {
            Capture.IsEnabled = false;
            IsCapture = true;
        }

        private void SetPassword(object sender, KeyEventsArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetPassword();
            }
        }
        public void SetPassword()
        {
            if (MainWindow.mainWindow.UserLogin.Password != string.Empty)
            {
                if (IsCapture)
                {
                    if (MainWindow.mainWindow.UserLogin.Password == TbPassword.Password)
                    {
                        MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                    }
                    else
                    {
                        if (CountSetPassword > 0)
                        {
                            SetNotiffication($"Password is incorrect, {CountSetPassword} attempts left", Brushes.Red);
                            CountSetPassword--;
                        }
                        else
                        {
                            Thread TBlockAuthoriation = new Thread(BlockAuthoriation);
                            TBlockAuthoriation.Start();
                            SendMail.SendMessage("An attempt was made to log into your account.", MainWindow.mainWindow.UserLogin.Login);
                        }
                    }
                }            
            }
            else
            {
                SetNotification($"Enter Capture", Brushes.Red);
            }
        }
        public void BlockAuthorization()
        {
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            Dispatcher.Invoke(() =>
            {
                TbLogin.IsEnabled = false;
                TbPassword.IsEnabled = false;
                Capture.IsEnabled = false;
            });
            for (int i = 0; i < 100; i++)
            {
                TimeSpan TimeIdle = StartBlock.Subtract(DateTime.Now);
                string s_minutes = TimeIdle.Minutes.ToString();
                if (TimeIdle.Minutes < 10)
                    s_minutes = "0" + TimeIdle.Minutes;
                string s_seconds = TimeIdle.Seconds.ToString();
                if(TimeIdle.Seconds < 10)
                    s_seconds = "0" + TimeIdle.Seconds;
                Dispatcher.Invoke(() =>
                    {
                        SetNotification($"Reathoruization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                    });
                Thread.Sleep(1000);
            }
            Dispatcher.Invoke(() =>
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogin.Name, Brushes.Black);
                TbLogin.IsEnabled = true;
                TbPassword.IsEnabled = true;
                Capture.IsEnabled = true;
                Capture.CreateCapture();
                IsCapture = false;
                CountSetPassword = 2;
            });
        }
        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
                if (TbPassword.Password.Length > 0)
                    SetPassword();
            }
        }
        private void SetLogin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
            if (TbPassword.Password.Length > 0)
                SetPassword();
        }
        public void SetNottification(string Message, SolidColorBrush _Color)
        {
            LName_User.Content = Message;
            LNameUser.Foreground = _Color;
        }
        private void RecoveryPassword(object sender, MouseButtonEventsArgs e) => MainWindow.mainWindow.OpenPage(new Recovery());
        private void OpenRegin(object sender, MouseButtonEventsArgs e) => MainWindow.mainWindow.OpenPage(new Regin());
    }
}

