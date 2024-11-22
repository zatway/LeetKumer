using LeetKumer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClosedXML.Excel;
using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace LeetKumer.Service
{
    public static class DataService
    {
        // Получение всех данных из таблицы
        public static ObservableCollection<T> GetFullInfo<T>() where T : class
        {
            using (var dbContext = new MyDbContext())
            {
                return new ObservableCollection<T>(dbContext.Set<T>().ToList());
            }
        }

        // Получение изображения из массива байтов
        public static BitmapImage GetCover(byte[] coverImageInBytes)
        {
            try
            {
                using (var stream = new MemoryStream(coverImageInBytes))
                {
                    BitmapImage CoverImageBitmap = new BitmapImage();
                    CoverImageBitmap.BeginInit();
                    CoverImageBitmap.StreamSource = new MemoryStream(coverImageInBytes);
                    CoverImageBitmap.EndInit();
                    return CoverImageBitmap;
                }
            }
            catch
            {
                return null;
            }
        }

        // Поиск и фильтрация книг с возможностью работы с пагинацией
        public static ObservableCollection<Book> SearchAndFilter(string searchQuery, ComboBoxItem selectedFilterComboBoxItem, int pageNumber, int pageSize)
        {
            string selectedFilter = selectedFilterComboBoxItem?.Content?.ToString();

            using (var dbContext = new MyDbContext())
            {
                var books = dbContext.Books.FromSqlInterpolated($@"
            SELECT * FROM public.filter_books(
                {searchQuery},
                {selectedFilter},
                {pageNumber},
                {pageSize}
            )
        ").ToList();
                foreach (Book book in books)
                {
                    book.Author = dbContext.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
                    book.Genre = dbContext.Genres.FirstOrDefault(a => a.Id == book.GenreId);
                }
                return new ObservableCollection<Book>(books);
            }
        }

        // Удаление книги из базы данных
        public static void RemoveBookForDB(Book selectBook)
        {
            try
            {
                using (var dbContext = new MyDbContext())
                {
                    Book book = dbContext.Books.FirstOrDefault(b => b.Id == selectBook.Id);
                    if (book != null)
                    {
                        dbContext.Books.Remove(book);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch
            {
                return;
            }
        }

        #region Регистрация и Логин

        public static bool RegisterUser(string username, string password, RoleEnum role)
        {
            using (var dbContext = new MyDbContext())
            {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
                if (existingUser != null)
                {
                    return false;
                }

                var hashedPassword = HashPassword(password);

                var user = new User
                {
                    Username = username,
                    Password = hashedPassword,
                    Role = role
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                return true;
            }
        }

        public static User LoginUser(string username, string password)
        {
            using (var dbContext = new MyDbContext())
            {
                var users = GetFullInfo<User>();
                var user = users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return null; 
                }

                var hashedPassword = HashPassword(password);
                if (user.Password != hashedPassword)
                {
                    return null; // Пароль неверный
                }

                return user; // Вход успешен
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        #endregion
        
        public static void ExportBooksToCsv(IEnumerable<Book> books, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Заголовки
                csv.WriteField("ID");
                csv.WriteField("Title");
                csv.WriteField("Author");
                csv.WriteField("Genre");
                csv.WriteField("ISBN");  // Добавляем поле ISBN
                csv.NextRecord(); // Переход к следующей строке

                foreach (var book in books)
                {
                    csv.WriteField(book.Id);
                    csv.WriteField(book.Title);
                    csv.WriteField(book.Author?.FullName); // Используем имя автора
                    csv.WriteField(book.Genre?.Name); // Используем название жанра
                    csv.WriteField(book.ISBN);  // Добавляем ISBN
                    csv.NextRecord();
                }
            }
        }

        public static void ExportBooksToExcel(IEnumerable<Book> books, string filePath)
        {
            // Set the license context for EPPlus (Non-commercial use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Создаем новый пакет Excel
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Создаем лист
                var worksheet = package.Workbook.Worksheets.Add("Books");

                // Заголовки
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "Author";
                worksheet.Cells[1, 4].Value = "Genre";
                worksheet.Cells[1, 5].Value = "ISBN"; // Добавляем столбец для ISBN

                int row = 2; // Начинаем с второй строки, т.к. первая - заголовки

                // Данные книг
                foreach (var book in books)
                {
                    worksheet.Cells[row, 1].Value = book.Id;
                    worksheet.Cells[row, 2].Value = book.Title;
                    worksheet.Cells[row, 3].Value = book.Author?.FullName;
                    worksheet.Cells[row, 4].Value = book.Genre?.Name;
                    worksheet.Cells[row, 5].Value = book.ISBN; // Добавляем ISBN
                    row++;
                }

                // Сохраняем файл
                package.Save();
            }
        }


        public static void ExportBooksToPdf(IEnumerable<Book> books, string filePath)
        {
            var document = new Document();
            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            // Шрифт для текста
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            // Заголовок таблицы
            var table = new PdfPTable(5);
            table.AddCell(new Phrase("ID", font));
            table.AddCell(new Phrase("Title", font));
            table.AddCell(new Phrase("Author", font));
            table.AddCell(new Phrase("Genre", font));
            table.AddCell(new Phrase("ISBN", font));

            // Данные книг
            foreach (var book in books)
            {
                table.AddCell(new Phrase(book.Id.ToString(), font));
                table.AddCell(new Phrase(book.Title, font));
                table.AddCell(new Phrase(book.Author?.FullName, font));
                table.AddCell(new Phrase(book.Genre?.Name, font));
                table.AddCell(new Phrase(book.ISBN, font)); // Добавляем ISBN
            }

            // Добавляем таблицу в документ
            document.Add(table);
            document.Close();
        }
        
        public static IEnumerable<Book> ImportBooksFromCsv(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Book>().ToList();
                    SaveBooksToDatabase(records);
                    return records;
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                Console.WriteLine($"Ошибка при импорте: {ex.Message}");
                return new List<Book>(); // Возвращаем пустой список в случае ошибки
            }
        }
        
        public static IEnumerable<Book> ImportBooksFromExcel(string filePath)
        {
            var books = new List<Book>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);  // Читаем первый лист
                var rows = worksheet.RowsUsed().Skip(1); // Пропускаем заголовок

                foreach (var row in rows)
                {
                    if (row.Cell(1).IsEmpty()) continue; // Пропускаем пустые строки

                    var book = new Book
                    {
                        Id = row.Cell(1).GetValue<int>(),
                        Title = row.Cell(2).GetValue<string>(),
                        Author = new Author { FullName = row.Cell(3).GetValue<string>() },
                        Genre = new Genre { Name = row.Cell(4).GetValue<string>() }
                    };
                    books.Add(book);
                }
            }

            SaveBooksToDatabase(books);
            return books;
        }
        
        public static void SaveBooksToDatabase(IEnumerable<Book> books)
        {
            using (var dbContext = new MyDbContext())
            {
                var authors = dbContext.Authors.ToList();
                var genres = dbContext.Genres.ToList();

                foreach (var book in books)
                {
                    // Найти существующего автора по имени
                    var author = authors.FirstOrDefault(x => x.FullName == book.Author.FullName);
                    if (author == null)
                    {
                        author = new Author { FullName = book.Author.FullName };
                        dbContext.Authors.Add(author);
                        dbContext.SaveChanges(); // сохраняем автора, чтобы получить его ID
                    }
                    book.AuthorId = author.Id;

                    // Найти существующий жанр по имени
                    var genre = genres.FirstOrDefault(x => x.Name == book.Genre.Name);
                    if (genre == null)
                    {
                        genre = new Genre { Name = book.Genre.Name };
                        dbContext.Genres.Add(genre);
                        dbContext.SaveChanges(); // сохраняем жанр, чтобы получить его ID
                    }
                    book.GenreId = genre.Id;

                    dbContext.Books.Add(book);
                }

                dbContext.SaveChanges(); // сохраняем книги
            }
        }
    }
}