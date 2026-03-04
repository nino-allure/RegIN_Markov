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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Org.BouncyCastle.Bcpg;

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

        #region SetConfirmPassword

        /// <summary>
        /// Метод повторного ввода пароля
        /// </summary>
        private void ConfirmPassword(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
            {
                // Вызываем метод повторения пароля
                ConfirmPassword();
            }
        }

        /// <summary>
        /// Метод повторного ввода пароля
        /// </summary>
        private void ConfirmPassword(object sender, System.Windows.RoutedEventArgs e) =>
            // Вызываем метод повторения пароля
            ConfirmPassword();

        /// <summary>
        /// Метод повторного ввода пароля
        /// </summary>
        public void ConfirmPassword(bool Pass = false)
        {
            // Записываем результат сравнения паролей в переменную
            BCorrectConfirmPassword = TbConfirmPassword.Password == TbPassword.Password;

            // Если пароль не совпадает с повторением пароля
            if (TbConfirmPassword.Password != TbPassword.Password)
            {
                // Выводим сообщение о том, что пароли не совпадают, красным цветом
                SetNotification("Passwords do not match", Brushes.Red);
            }
            else
            {
                // Если пароли совпадают, выводим пустое сообщение чёрным цветом
                SetNotification("", Brushes.Black);

                // Если проверка идёт не из метода проверки пароля
                // Исключаем зацикливание методов
                if (!Pass)
                {
                    // Вызываем проверку пароля
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
                MainWindow.mainWindow.UserLogin.Image = File.ReadyAllBytes(Directory.GetCurrentDirectory() + @"\IUser.jpg");
            MainWindow.mainWindow.UserLogin.DateUpdate = DateTime.Now;
            MainWindow.mainWindow.UserLogin.DateUpdate = DateTime.Now;
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
            // Если статус открытия диалогового окна true
            if (FileDialogImage.ShowDialog() == true)
            {
                // конвертируем размер изображения
                using (Imaging.Image image = Imaging.Image.Load(FileDialogImage.FileName))
                {
                    // создаём ширину изображения
                    int NewWidth = 0;
                    // Создаём высоту изображения
                    int NewHeight = 0;

                    // проверяем какая из сторон больше
                    if (image.Width > image.Height)
                    {
                        // Рассчитываем новую ширину относительно высоты
                        NewWidth = (int)(image.Width * (256f / image.Height));
                        // Задаём высоту изображения
                        NewHeight = 256;
                    }
                    else
                    {
                        // Задаём ширину изображения
                        NewWidth = 256;
                        // Рассчитываем новую высоту относительно ширины
                        NewHeight = (int)(image.Height * (256f / image.Width));
                    }

                    // Изменяем изображение
                    image.Resize(NewWidth, NewHeight);

                    // Сохраняем изображение
                    image.Save("IUser.jpg");
                }
                using (Imaging.RasterImage rasterImage = (Imaging.RasterImage).Imaging.Image.Load("IUser.jpg"))
                {
                    if (!rasterImage.IsCached)
                    {
                        rasterImage.CacheData();
                    }
                    int X = 0;
                    int Width = 256;
                    int Y = 0;
                    int Height = 256;
                    if (rasterImage.Width > rasterImage.Height)
                        X = (int)((rasterImage.Width - 256f) / 2);
                    else
                        Y = (int)((rasterImage.Height - 256f) / 2);
                    Imaging.Rectangle rectangle = new Imaging.Rectangle(X, Y, Width, Height);
                    rasterImage.Crop(rectangle);
                    rasterImage.Save("IUser.jpg");
                }
                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri(Directory.GetCurrentDirrectory() + @"\IUser.jpg"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                };
                IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
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
