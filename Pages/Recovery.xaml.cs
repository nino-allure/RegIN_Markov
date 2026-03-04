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
using RegIN_Markov.Pages;

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
            if (LNameUser.Content != "")
            {
                LNameUser.Content = "";

                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                StartAnimation.Completed += delegate
                {
                    User.Source = new BitmapImage(new Uri("pack://application:,,,/RegIN_Markov;component/Images/user.jpeg")); // Исправлен путь

                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                    User.BeginAnimation(Image.OpacityProperty, EndAnimation);
                };

                User.BeginAnimation(Image.OpacityProperty, StartAnimation);
            }

            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);
        }
        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
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
            if (isCapture)
            {
                if (MainWindow.mainWindow.UserLogin.Password != String.Empty)
                {
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                    StartAnimation.Completed += delegate
                    {
                        User.Source = new BitmapImage(new Uri("pack://application:,,,/RegIN_Markov;component/Images/mail.jpg")); // Исправлен путь

                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                        User.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    User.BeginAnimation(Image.OpacityProperty, StartAnimation);

                    SetNotification("An email has been sent to your email.", Brushes.Black);

                    MainWindow.mainWindow.UserLogin.CreateNewPassword();
                }
            }
        }
        private void OpenLogin(object sender, MouseButtonEventArgs e) // Исправлено: RoutedEventArgs -> MouseButtonEventArgs
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}