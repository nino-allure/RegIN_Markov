using System;
using System.Collections.Generic;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Org.BouncyCastle.Bcpg;
using Imaging = Aspose.Imaging;
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
            FileDialogImage.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg"; // Исправлено
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
            if (e.Key == Key.Enter)
            {
                SetLogin();
            }
        }

        private void SetLogin(object sender, System.Windows.RoutedEventArgs e) =>
            SetLogin();

        public void SetLogin()
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._-]{4,}@[a-zA-Z0-9._-]{2,}\.[a-zA-Z0-9._-]{2,}$");

            BCorrectLogin = regex.IsMatch(TbLogin.Text);

            if (regex.IsMatch(TbLogin.Text) == true)
            {
                SetNotification("", Brushes.Black);
                MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text); // Исправлено: UserLogIn -> UserLogin
            }
            else
            {
                SetNotification("Invalid login", Brushes.Red);
            }

            OnRegin(); // Исправлено: было OnRegion()
        }
        #region SetPassword

        private void SetPassword(object sender, System.Windows.RoutedEventArgs e) =>
            SetPassword();

        private void SetPassword(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetPassword();
        }

        public void SetPassword()
        {
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[!@#$%&?*\-_=])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%&?*\-_=]{10,}");

            BCorrectPassword = regex.IsMatch(TbPassword.Password);

            if (regex.IsMatch(TbPassword.Password) == true)
            {
                SetNotification("", Brushes.Black);

                if (TbConfirmPassword.Password.Length > 0)
                    ConfirmPassword(true);

                OnRegin(); // Исправлено: было OnRegion()
            }
            else
            {
                SetNotification("Invalid password", Brushes.Red);
            }
        }
        #endregion

        #region SetConfirmPassword

        private void ConfirmPassword(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmPassword();
            }
        }

        private void ConfirmPassword(object sender, System.Windows.RoutedEventArgs e) =>
            ConfirmPassword();

        public void ConfirmPassword(bool Pass = false)
        {
            BCorrectConfirmPassword = TbConfirmPassword.Password == TbPassword.Password;

            if (TbConfirmPassword.Password != TbPassword.Password)
            {
                SetNotification("Passwords do not match", Brushes.Red);
            }
            else
            {
                SetNotification("", Brushes.Black);

                if (!Pass)
                {
                    SetPassword();
                }
            }
        }

        #endregion

        void OnRegin()
        {
            if (!BCorrectLogin)
                return;
            if (TbName.Text.Length == 0)
                return;
            if (!BCorrectPassword)
                return;
            if (!BCorrectConfirmPassword)
                return;
            MainWindow.mainWindow.UserLogin.Login = TbLogin.Text;
            MainWindow.mainWindow.UserLogin.Password = TbPassword.Password;
            MainWindow.mainWindow.UserLogin.Name = TbName.Text;
            if (BSetImages)
                MainWindow.mainWindow.UserLogin.Image = File.ReadAllBytes(Directory.GetCurrentDirectory() + @"\IUser.jpg"); // Исправлено
            MainWindow.mainWindow.UserLogin.DateUpdate = DateTime.Now;
            MainWindow.mainWindow.UserLogin.DateCreate = DateTime.Now; // Исправлено: добавлено DateCreate
            MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Regin));
        }

        private void SetName(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsLetter(e.Text, 0));
        }
        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
        }
        private void SelectImage(object sender, MouseButtonEventArgs e)
        {
            if (FileDialogImage.ShowDialog() == true)
            {
                using (Imaging.Image image = Imaging.Image.Load(FileDialogImage.FileName))
                {
                    int NewWidth = 0;
                    int NewHeight = 0;

                    if (image.Width > image.Height)
                    {
                        NewWidth = (int)(image.Width * (256f / image.Height));
                        NewHeight = 256;
                    }
                    else
                    {
                        NewWidth = 256;
                        NewHeight = (int)(image.Height * (256f / image.Width));
                    }
                    }

                    using (System bitmap = new System.Drawing.Bitmap(image, NewWidth, NewHeight))
                    {
                        int X = 0;
                        int Y = 0;
                        if (bitmap.Width > 256)
                            X = (bitmap.Width - 256) / 2;
                        if (bitmap.Height > 256)
                            Y = (bitmap.Height - 256) / 2;

                        System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(X, Y, 256, 256);
                        using (System.Drawing.Bitmap croppedBitmap = bitmap.Clone(cropRect, bitmap.PixelFormat))
                        {
                            croppedBitmap.Save("IUser.jpg");
                        }
                    }
                }

                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    User.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\IUser.jpg")); // Исправлено: IUser -> User, GetCurrentDirrectory -> GetCurrentDirectory
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    User.BeginAnimation(Image.OpacityProperty, EndAnimation); // Исправлено: IUser -> User
                };
                User.BeginAnimation(Image.OpacityProperty, StartAnimation); // Исправлено: IUser -> User
                BSetImages = true;
            }
            else
                BSetImages = false;
        }
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}