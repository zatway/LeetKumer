using LeetKumer.Service;
using LeetKumer.Models;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LeetKumer.Views;

namespace LeetKumer.ViewModels
{
    public class AutorizationViewModel : BaseViewModel
    {
        private readonly User _model;
        private string _captchaInput;

        public AutorizationViewModel()
        {
            _model = new User();
            GenerateCaptcha();

            LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
            CloseCommand = new RelayCommand(CloseWindow);
        }

        // Свойства
        public string Username
        {
            get => _model.Username;
            set
            {
                _model.Username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _model.Password;
            set
            {
                _model.Password = value;
                OnPropertyChanged();
            }
        }
        
        public string CaptchaInput
        {
            get => _captchaInput;
            set
            {
                _captchaInput = value;
                OnPropertyChanged();
            }
        }
        
        public RoleEnum Role
        {
            get => _model.Role;
            set
            {
                _model.Role = value;
                OnPropertyChanged();
            }
        }

        private string _captchaText;
        private BitmapImage _captchaImage;

        public BitmapImage CaptchaImage
        {
            get => _captchaImage;
            set
            {
                _captchaImage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }
        
        public ICommand CloseCommand { get; private set; }

        private void CloseWindow(object parameter)
        {
            // Команда для закрытия окна
            var window = parameter as Window;
            window?.Close();
        }  
        private void ExecuteLogin(object parameter)
        {
            if (CaptchaInput != _captchaText)
            {
                MessageBox.Show("Капча введена не корректно", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GenerateCaptcha();
                return;
            }

            if (string.IsNullOrEmpty(Username) || 
                string.IsNullOrEmpty(Password) || 
                string.IsNullOrEmpty(CaptchaInput))
            {
                MessageBox.Show("Заполнены не все поля", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
    
            User user = DataService.LoginUser(Username, Password);
            if (user != null)
            {
                var viewModel = new BookCatalogViewModel(user);

                var window = parameter as Window;
                window?.Close(); 

                var newWindow = new MainPageWindow()
                {
                    DataContext = viewModel,
                };
                newWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пользователя не существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExecuteRegister(object parameter)
        {
            if (CaptchaInput != _captchaText)
            {
                MessageBox.Show("Капча введена не корректно", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GenerateCaptcha();
                return;
            }
            
            bool result = DataService.RegisterUser(Username, Password, Role);
            if (!result)
            {
                MessageBox.Show("Такой пользователь уже существует", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Регистрация прошла успешно'\n'Войдите в систему", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GenerateCaptcha()
        {
            _captchaText = Guid.NewGuid().ToString("N").Substring(0, 6); // Генерация текста капчи
            CaptchaImage = GenerateCaptchaImage(_captchaText);
        }

        private BitmapImage GenerateCaptchaImage(string text)
{
    var random = new Random();
    var bitmap = new System.Drawing.Bitmap(200, 80);

    using (var g = System.Drawing.Graphics.FromImage(bitmap))
    {
        // Заливка фона случайным светлым цветом
        g.Clear(System.Drawing.Color.FromArgb(random.Next(200, 255), random.Next(200, 255), random.Next(200, 255)));

        // Рисование случайных линий
        for (int i = 0; i < 10; i++)
        {
            var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255)));
            g.DrawLine(pen, random.Next(bitmap.Width), random.Next(bitmap.Height), random.Next(bitmap.Width), random.Next(bitmap.Height));
        }

        // Рисование случайных точек
        for (int i = 0; i < 100; i++)
        {
            bitmap.SetPixel(random.Next(bitmap.Width), random.Next(bitmap.Height), System.Drawing.Color.FromArgb(random.Next(150, 255), random.Next(150, 255), random.Next(150, 255)));
        }

        // Рисование текста капчи
        var font = new System.Drawing.Font("Arial", 28, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
        var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(random.Next(0, 150), random.Next(0, 150), random.Next(0, 150)));

        // Смещение текста для создания кривой линии
        var textPath = new System.Drawing.Drawing2D.GraphicsPath();
        textPath.AddString(text, font.FontFamily, (int)font.Style, font.Size, new System.Drawing.Point(random.Next(10, 30), random.Next(10, 30)), new System.Drawing.StringFormat());
        var matrix = new System.Drawing.Drawing2D.Matrix();
        matrix.RotateAt(random.Next(-20, 20), new System.Drawing.PointF(bitmap.Width / 2, bitmap.Height / 2));
        textPath.Transform(matrix);

        g.FillPath(brush, textPath);
    }

    // Конвертация в BitmapImage
    var bitmapImage = new BitmapImage();
    using (var memory = new System.IO.MemoryStream())
    {
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        memory.Position = 0;

        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
    }

    return bitmapImage;
}

    }
}
