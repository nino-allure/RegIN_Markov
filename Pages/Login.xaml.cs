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
    }
}

