using LeetKumer.Models;
using LeetKumer.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;

namespace LeetKumer.ViewModels
{
    public class BookCatalogViewModel : INotifyPropertyChanged
    {
        public BookCatalogViewModel(User user)
        {
            StartSearchCommand = new RelayCommand(o =>
            {
                BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
            });

            OpenCardBookCommand = new RelayCommand(o =>
            {
                WindowControlService.OpenWindowCardBook(SelectedBook);
                UpdateBookList();
            });

            OpenWindowAddBookCommand = new RelayCommand(o =>
            {
                WindowControlService.OpenWindowAddBook();
                UpdateBookList();
            });

            RemoveBookCommand = new RelayCommand(o =>
            {
                if (SelectedBook != null)
                {
                    DataService.RemoveBookForDB(SelectedBook);
                    BooksList.Remove(SelectedBook);
                    OnPropertyChanged(nameof(BooksList));
                }
                else
                    MessageBox.Show("Книга не выбрана");
                UpdateBookList();
            });

            PreviousPageCommand = new RelayCommand(o => { PreviousPage(); });

            NextPageCommand = new RelayCommand(o => { NextPage(); });

            // Команды для импорта и экспорта
            ExportCsvCommand = new RelayCommand(o => ExportToCsv());
            ExportExcelCommand = new RelayCommand(o => ExportToExcel());
            ExportPdfCommand = new RelayCommand(o => ExportToPdf());
            ImportCsvCommand = new RelayCommand(o => ImportFromCsv());
            ImportExcelCommand = new RelayCommand(o => ImportFromExcel());

            _pageNumber = 1;
            PageSize = 5;
            UpdateBookList();
        }

        public ICommand OpenCardBookCommand { get; }
        public ICommand OpenWindowAddBookCommand { get; }
        public ICommand RemoveBookCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand StartSearchCommand { get; }

        // Команды для импорта/экспорта
        public ICommand ExportCsvCommand { get; }
        public ICommand ExportExcelCommand { get; }
        public ICommand ExportPdfCommand { get; }
        public ICommand ImportCsvCommand { get; }
        public ICommand ImportExcelCommand { get; }

        private Book _selectedBook;

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged(nameof(SelectedBook));
                }
            }
        }

        private ComboBoxItem _selectedFilter;

        public ComboBoxItem SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    if (value.Content.ToString() == "Убрать фильтры")
                        _selectedFilter = null;
                    else
                        _selectedFilter = value;
                    BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
                    OnPropertyChanged(nameof(SelectedFilter));
                }
            }
        }

        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        private ObservableCollection<Book> _booksList;

        public ObservableCollection<Book> BooksList
        {
            get { return _booksList; }
            set
            {
                if (_booksList != value)
                {
                    _booksList = value;
                    OnPropertyChanged(nameof(BooksList));
                }
            }
        }

        public readonly int PageSize;

        private int _pageNumber;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
                    OnPropertyChanged(nameof(PageNumber));
                }
            }
        }

        private void NextPage()
        {
            PageNumber++;
        }

        private void PreviousPage()
        {
            if (PageNumber > 1)
            {
                PageNumber--;
            }
        }

        private void UpdateBookList()
        {
            BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
        }

        private void ExportToCsv()
        {
            var filePath = SaveFileDialoghelper("csv");
            if (string.Empty != filePath)
            {
                ExportInfo.ExportToCsv(filePath);
                MessageBox.Show("Экспорт в CSV завершен.");
            }
            OnPropertyChanged(nameof(BooksList));
        }

        private void ExportToExcel()
        {
            var filePath = SaveFileDialoghelper("xlsx");
            if (string.Empty != filePath)
            {
                ExportInfo.ExportToExcel(filePath);
                MessageBox.Show("Экспорт в Excel завершен.");
            }
            OnPropertyChanged(nameof(BooksList));
        }

        private void ExportToPdf()
        {
            var filePath = SaveFileDialoghelper("pdf");
            if (string.Empty != filePath)
            {
                ExportInfo.ExportToPDF(filePath);
                MessageBox.Show("Экспорт в PDF завершен.");
            }
        }

        private void ImportFromCsv()
        {
            var filePath = OpenFileDialoghelper("csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                var importedBooks = ImportInfo.ImportFromCsv(filePath);
                if (importedBooks != null && importedBooks.Any())
                {
                    foreach (var book in importedBooks)
                    {
                        BooksList.Add(book);
                    }
                    MessageBox.Show("Импорт из CSV завершен.");
                }
            }
            OnPropertyChanged(nameof(BooksList));
        }

        private void ImportFromExcel()
        {
            var filePath = OpenFileDialoghelper("xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                var importedBooks = ImportInfo.ImportFromExcel(filePath);
                if (importedBooks != null && importedBooks.Any())
                {
                    foreach (var book in importedBooks)
                    {
                        BooksList.Add(book);
                    }

                    MessageBox.Show("Импорт из Excel завершен.");
                }

                OnPropertyChanged(nameof(BooksList));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string OpenFileDialoghelper(string extensions)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = $"Файлы (*.{extensions})|*.{extensions}"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        private string SaveFileDialoghelper(string extensions)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = $"Файлы (*.{extensions})|*.{extensions}",
                DefaultExt = extensions
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}