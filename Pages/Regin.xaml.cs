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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Org.BouncyCastle.Bcpg;
using AsposeImage = Aspose.Imaging.Image;
using DrawingImage = System.Drawing.Image;
using System.Drawing;
using System.Drawing.Imaging;
using WpfImage = System.Windows.Controls.Image; // Алиас для WPF Image контрола

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
            FileDialogImage.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
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
            SetNotification("Login already in use", System.Windows.Media.Brushes.Red);
            BCorrectLogin = false;
        }

        private void InCorrectLogin() =>
            SetNotification("", System.Windows.Media.Brushes.Black);

        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetLogin();
            }
        }

        private void SetLogin(object sender, RoutedEventArgs e) =>
            SetLogin();

        public void SetLogin()
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._-]{4,}@[a-zA-Z0-9._-]{2,}\.[a-zA-Z0-9._-]{2,}$");

            BCorrectLogin = regex.IsMatch(TbLogin.Text);

            if (regex.IsMatch(TbLogin.Text) == true)
            {
                SetNotification("", System.Windows.Media.Brushes.Black);
                MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text);
            }
            else
            {
                SetNotification("Invalid login", System.Windows.Media.Brushes.Red);
            }

            OnRegin();
        }

        #region SetPassword

        private void SetPassword(object sender, RoutedEventArgs e) =>
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
                SetNotification("", System.Windows.Media.Brushes.Black);

                if (TbConfirmPassword.Password.Length > 0)
                    ConfirmPassword(true);

                OnRegin();
            }
            else
            {
                SetNotification("Invalid password", System.Windows.Media.Brushes.Red);
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

        private void ConfirmPassword(object sender, RoutedEventArgs e) =>
            ConfirmPassword();

        public void ConfirmPassword(bool Pass = false)
        {
            BCorrectConfirmPassword = TbConfirmPassword.Password == TbPassword.Password;

            if (TbConfirmPassword.Password != TbPassword.Password)
            {
                SetNotification("Passwords do not match", System.Windows.Media.Brushes.Red);
            }
            else
            {
                SetNotification("", System.Windows.Media.Brushes.Black);

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

            if (BSetImages && File.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "IUser.jpg")))
                MainWindow.mainWindow.UserLogin.Image = File.ReadAllBytes(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "IUser.jpg"));

            MainWindow.mainWindow.UserLogin.DateUpdate = DateTime.Now;
            MainWindow.mainWindow.UserLogin.DateCreate = DateTime.Now;
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
                try
                {
                    string outputPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "IUser.jpg");

                    // Используем алиас для Aspose.Imaging.Image
                    using (AsposeImage image = AsposeImage.Load(FileDialogImage.FileName))
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

                        // Сохраняем во временный файл
                        string tempFile = System.IO.Path.GetTempFileName() + ".jpg";
                        image.Save(tempFile);

                        // Загружаем через System.Drawing для обрезки
                        using (System.Drawing.Bitmap originalBitmap = new System.Drawing.Bitmap(tempFile))
                        {
                            // Создаем измененный размер
                            using (System.Drawing.Bitmap resizedBitmap = new System.Drawing.Bitmap(originalBitmap, new System.Drawing.Size(NewWidth, NewHeight)))
                            {
                                int X = 0;
                                int Y = 0;

                                if (resizedBitmap.Width > 256)
                                    X = (resizedBitmap.Width - 256) / 2;
                                if (resizedBitmap.Height > 256)
                                    Y = (resizedBitmap.Height - 256) / 2;

                                // Создаем прямоугольник для обрезки
                                System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(X, Y, 256, 256);

                                // Обрезаем изображение
                                using (System.Drawing.Bitmap croppedBitmap = resizedBitmap.Clone(cropRect, resizedBitmap.PixelFormat))
                                {
                                    // Сохраняем в итоговый файл
                                    croppedBitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }

                        // Удаляем временный файл
                        if (File.Exists(tempFile))
                            File.Delete(tempFile);
                    }

                    // Анимация появления
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += (s, ev) =>
                    {
                        if (File.Exists(outputPath))
                        {
                            // Загружаем изображение в WPF Image
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(outputPath, UriKind.Absolute);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmap.EndInit();
                            bitmap.Freeze();

                            // User - это ваш WPF Image контрол
                            User.Source = bitmap;

                            DoubleAnimation EndAnimation = new DoubleAnimation();
                            EndAnimation.From = 0;
                            EndAnimation.To = 1;
                            EndAnimation.Duration = TimeSpan.FromSeconds(1.2);

                            // Явно указываем WpfImage.OpacityProperty
                            User.BeginAnimation(WpfImage.OpacityProperty, EndAnimation);
                        }
                    };

                    // Явно указываем WpfImage.OpacityProperty
                    User.BeginAnimation(WpfImage.OpacityProperty, StartAnimation);
                    BSetImages = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}\n\nStack trace: {ex.StackTrace}", "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    BSetImages = false;
                }
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