using System.ComponentModel;
using System.Windows.Input;
using LeetKumer.Service;

namespace LeetKumer.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand ShowBookCatalogViewCommand { get; }

        public MainPageWindowViewModel()
        {
            ShowBookCatalogViewCommand = new RelayCommand(o => ShowBookCatalogView());
            ShowBookCatalogView();
        }

        public void ShowBookCatalogView()
        {
            
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
