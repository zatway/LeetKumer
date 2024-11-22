using System.Collections.ObjectModel;
using System.IO;
using LeetKumer.Models;
using OfficeOpenXml;

namespace LeetKumer.Service;

public static class ImportInfo
{
public static List<Book> ImportFromCsv(string filePath)
{
    var newBooks = new List<Book>();
    var lines = File.ReadAllLines(filePath);
    var authors = DataService.GetFullInfo<Author>() ?? new ObservableCollection<Author>();
    var genres = DataService.GetFullInfo<Genre>() ?? new ObservableCollection<Genre>();
    var books = DataService.GetFullInfo<Book>() ?? new ObservableCollection<Book>();

    using (var dbContext = new MyDbContext())
    {
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');

            if (int.TryParse(values[0], out int idBook) && !string.IsNullOrWhiteSpace(values[1]))
            {
                // Проверка и парсинг даты
                if (!DateTimeOffset.TryParse(values[3], out DateTimeOffset yearOfManufactureOffset))
                    continue;

                var yearOfManufacture = yearOfManufactureOffset.UtcDateTime;
                var author = authors.FirstOrDefault(a => a.FullName == values[1]);
                var genre = genres.FirstOrDefault(g => g.Name == values[4]);
                var book = new Book
                {
                    Title = values[0]?.Trim() ?? string.Empty,
                    ISBN = values[2]?.Trim() ?? string.Empty,
                    YearOfManufacture = yearOfManufacture,
                    Author = author,
                    Genre = genre,
                };

                dbContext.Books.Add(book);
                newBooks.Add(book);
            }
        }
    }

    return newBooks;
}


    public static List<Book> ImportFromExcel(string filePath)
    {
        var authors = DataService.GetFullInfo<Author>() ?? new ObservableCollection<Author>();
        var genres = DataService.GetFullInfo<Genre>() ?? new ObservableCollection<Genre>();
        var books = DataService.GetFullInfo<Book>() ?? new ObservableCollection<Book>();
        
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<Book> newBooks = new List<Book>();

            var worksheet = package.Workbook.Worksheets[0]; // Первая вкладка Excel
            int rows = worksheet.Dimension.Rows; // Количество строк

            for (int i = 2; i <= rows; i++) // Пропускаем первую строку (заголовки)
            {
                // Чтение данных из ячеек
                string title = worksheet.Cells[i, 1].Text?.Trim() ?? string.Empty;
                Author author = authors.FirstOrDefault(a => a.FullName == worksheet.Cells[i, 2].Text?.Trim());
                string isbn = worksheet.Cells[i, 3].Text?.Trim() ?? string.Empty;
                string yearOfManufactureText = worksheet.Cells[i, 4].Text?.Trim() ?? string.Empty;
                Genre genre = genres.FirstOrDefault(a => a.Name == worksheet.Cells[i, 5].Text?.Trim());

                if (!DateTimeOffset.TryParse(yearOfManufactureText, out DateTimeOffset yearOfManufactureOffset))
                    continue;

                var yearOfManufacture = yearOfManufactureOffset.UtcDateTime;
                var book = new Book
                {
                    Title = title,
                    ISBN = isbn,
                    YearOfManufacture = yearOfManufacture,
                    Author = author,
                    Genre = genre
                };

                newBooks.Add(book);


            }

            return newBooks;
        }
    }
}