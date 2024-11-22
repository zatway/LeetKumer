using LeetKumer.Models;
using LeetKumer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

namespace LeetKumer.ViewModels
{
    /// <summary>
    /// Добавление новой книги
    /// </summary>
    internal class AddNewBookViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса AddNewBookViewModel
        /// </summary>
        public AddNewBookViewModel()
        {
            LoadAuthors();
            LoadGenres();
        }
        private ICommand _selectFileCommand;
        /// <summary>
        /// Команда для выбора фотографии обложки книги
        /// </summary>
        public ICommand SelectFileCommand
        {
            get
            {
                if (_selectFileCommand == null)
                {
                    _selectFileCommand = new RelayCommand(param => SelectFileCommandExecute());
                }
                return _selectFileCommand;
            }
        }

        private ICommand _addBookCommand;
        /// <summary>
        /// Команда для добавления книги
        /// </summary>
        public ICommand AddBookCommand
        {
            get
            {
                if (_addBookCommand == null)
                {
                    _addBookCommand = new RelayCommand(param => AddNewBook());
                }
                return _addBookCommand;
            }
        }

        /// <summary>
        /// Команда для проверки нажатия на кнопку "Автора нету в списке"
        /// </summary>
        private ICommand _authorCheckBoxCommand;
        public ICommand AuthorCheckBoxCommand
        {
            get
            {
                if (_authorCheckBoxCommand == null)
                {
                    _authorCheckBoxCommand = new RelayCommand(param => ExecuteAuthorCheckBoxCommand());
                }
                return _authorCheckBoxCommand;
            }
        }

        /// <summary>
        /// Команда для проверки нажатия на кнопку "Жанра нету в списке"
        /// </summary>
        private ICommand _genreCheckBoxCommand;
        public ICommand GenreCheckBoxCommand
        {
            get
            {
                if (_genreCheckBoxCommand == null)
                {
                    _genreCheckBoxCommand = new RelayCommand(param => ExecuteGenreCheckBoxCommand());
                }
                return _genreCheckBoxCommand;
            }
        }

        private bool _isAuthorNotInListChecked;
        public bool IsAuthorNotInListChecked
        {
            get => _isAuthorNotInListChecked;
            set
            {
                if (_isAuthorNotInListChecked != value)
                {
                    _isAuthorNotInListChecked = value;
                    OnPropertyChanged(nameof(IsAuthorNotInListChecked));
                    if (_isAuthorNotInListChecked)
                    {
                        AuthorCheckBoxCommand.Execute(null);
                    }
                }
            }
        }

        private bool _isGenreNotInListChecked;
        public bool IsGenreNotInListChecked
        {
            get => _isGenreNotInListChecked;
            set
            {
                if (_isGenreNotInListChecked != value)
                {
                    _isGenreNotInListChecked = value;
                    OnPropertyChanged(nameof(IsGenreNotInListChecked));
                    if (_isGenreNotInListChecked)
                    {
                        GenreCheckBoxCommand.Execute(null);
                    }
                }
            }
        }
        private ObservableCollection<Genre> _genres;
        /// <summary>
        /// Список который хранит список жанров
        /// </summary>
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        private ObservableCollection<Author> _authors;
        /// <summary>
        /// Список который хранит список авторов
        /// </summary>
        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set
            {
                if (value != _authors)
                {
                    _authors = value;
                    OnPropertyChanged(nameof(Authors));
                }
            }
        }

        private Author _author;
        public Author Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        private Genre _genre;
        public Genre Genre
        {
            get { return _genre; }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged(nameof(Genre));
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        private string _isbn;
        public string ISBN
        {
            get { return _isbn; }
            set
            {
                if (_isbn != value)
                {
                    _isbn = value;
                    OnPropertyChanged(nameof(ISBN));
                }
            }
        }

        private DateTime _yearOfManufacture;
        public DateTime YearOfManufacture
        {
            get { return _yearOfManufacture; }
            set
            {
                if (_yearOfManufacture != value)
                {
                    _yearOfManufacture = value;
                    OnPropertyChanged(nameof(YearOfManufacture));
                }
            }
        }

        private byte[] _coverImageByte;
        public byte[] CoverImageByte
        {
            get => _coverImageByte;
            set
            {
                if (_coverImageByte != value)
                {
                    _coverImageByte = value;
                    OnPropertyChanged(nameof(CoverImageByte));
                }
            }
        }

        private BitmapImage _coverImageInBytes;
        public BitmapImage CoverImageBitmap
        {
            get => _coverImageInBytes;
            set
            {
                if (_coverImageInBytes == null || _coverImageInBytes != value)
                {
                    _coverImageInBytes = DataService.GetCover(_coverImageByte);
                    OnPropertyChanged(nameof(CoverImageBitmap));
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private void LoadGenres() => Genres = DataService.GetFullInfo<Genre>();

        private void LoadAuthors() => Authors = DataService.GetFullInfo<Author>();

        /// <summary>
        /// Добавление новой книги
        /// </summary>
        private void AddNewBook()
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Не все поля заполнены");
            }
            else
            {
                try
                {
                    using (var dbContext = new MyDbContext())
                    {
                        // Проверяем, существует ли уже книга с таким названием и ISBN
                        bool bookISBNExists = dbContext.Books.Any(b => b.Title == _title || b.ISBN == _isbn);
                        bool bookAuthorAndTitleExists = dbContext.Books.Any(b => b.Title == _title && b.AuthorId == Author.Id);
                        if (bookISBNExists)
                        {
                            MessageBox.Show("Книга с таким ISBN уже существует в базе данных");
                            return; // Прерываем добавление книги
                        }
                        if (bookAuthorAndTitleExists)
                        {
                            MessageBox.Show("Книга с таким автором и названием уже существует в базе данных");
                            return; // Прерываем добавление книги
                        }

                        CoverImage coverImage = new CoverImage()
                        {
                            ImageData = CoverImageByte
                        };

                        // Добавляем coverImage в контекст и сохраняем изменения
                        dbContext.CoverImages.Add(coverImage);
                        dbContext.SaveChanges();

                        // Создаем новую книгу, используя назначенные Id
                        Book newBook = new Book()
                        {
                            Title = _title,
                            YearOfManufacture = _yearOfManufacture.ToUniversalTime(),
                            ISBN = _isbn,
                            AuthorId = _author.Id,
                            //Author = _author,
                            Description = _description,
                            GenreId = _genre.Id,
                            //Genre = _genre,
                            CoverImageId = coverImage.Id,
                            //CoverImage = coverImage,
                        };

                        dbContext.Books.Add(newBook);
                        dbContext.SaveChanges();

                        MessageBox.Show("Книга успешно сохранена");
                        CloseWindow();
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Выводим внутреннее исключение, если оно есть
                    var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {innerExceptionMessage}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Валидация для добавления ккниги
        /// </summary>
        private bool ValidateInputs()
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   YearOfManufacture != default(DateTime) &&
                   !string.IsNullOrWhiteSpace(ISBN) &&
                   Author != null &&
                   Genre != null && CoverImageBitmap != null;
        }

        /// <summary>
        /// Обработчик добавления фотографии обложки книги
        /// </summary>
        private void SelectFileCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                CoverImageByte = File.ReadAllBytes(selectedImagePath);
                CoverImageBitmap = new BitmapImage(new Uri(selectedImagePath));
            }
        }
        /// <summary>
        /// Свойство для отслеживания открытых окон
        /// </summary>
        public Action CloseWindow { get; set; }

        /// <summary>
        /// Обработчикк событий, который открывает окно добавления автора
        /// </summary>
        private void ExecuteAuthorCheckBoxCommand()
        {
            if (IsAuthorNotInListChecked)
            {
                WindowControlService.OpenWindowAddAuthor();
                LoadAuthors();
            }
            IsAuthorNotInListChecked = false;
        }

        /// <summary>
        /// Обработчикк событий, который открывает окно добавления жанра
        /// </summary>
        private void ExecuteGenreCheckBoxCommand()
        {
            if (IsGenreNotInListChecked)
            {
                WindowControlService.OpenWindowAddGenre();
                LoadGenres();
            }
            IsGenreNotInListChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

