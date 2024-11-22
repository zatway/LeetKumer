using LeetKumer.Models;
using LeetKumer.Service;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LeetKumer.ViewModels
{
    public class AddAutorsViewModel : INotifyPropertyChanged
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

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }

        private string _patronymic;
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                if (_patronymic != value)
                {
                    _patronymic = value;
                    OnPropertyChanged(nameof(Patronymic));
                }
            }
        }
 

        public Action CloseWindow { get; set; }


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

        public void SaveExecute()
        {
            if (Validation())
            {
                string fullName = $"{Name} {Patronymic} {Surname}";
                using (var dbContext = new MyDbContext())
                {
                    bool authorExists = dbContext.Authors.Any(a => a.FullName == fullName);
                    if (authorExists)
                    {
                        MessageBox.Show("Такой автор уже добавлен");
                        return;
                    }

                    Author newAuthor = new Author()
                    {
                        FullName = fullName,
                    };
                    dbContext.Authors.Add(newAuthor);
                    dbContext.SaveChanges();
                }
                CloseWindow();
            }
        }
        bool Validation() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Surname) && !string.IsNullOrWhiteSpace(Patronymic);
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
