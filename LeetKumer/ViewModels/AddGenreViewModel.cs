using LeetKumer.Models;
using LeetKumer.Service;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LeetKumer.ViewModels
{
    internal class AddGenreViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => SaveExecute());
                }
                return _saveCommand;
            }
        }   

        public Action CloseWindow { get; set; }
        public void SaveExecute()
        {
            if (Validation())
            {
                using (var dbContext = new MyDbContext())
                {
                    bool genreExists = dbContext.Genres.Any(a => a.Name == this.Name);
                    if (genreExists)
                    {
                        MessageBox.Show("Такой автор уже добавлен");
                        return;
                    }
                    Genre newGenre = new Genre()
                    {
                        Name = Name,
                    };
                    dbContext.Genres.Add(newGenre);
                    dbContext.SaveChanges();
                }
                CloseWindow();
            }
            else
            {
                MessageBox.Show("Поле заполнено некорректно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool Validation() => !string.IsNullOrWhiteSpace(Name);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
