using System.IO;
using System.Text;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LeetKumer.Models;
using OfficeOpenXml;
using Author = DocumentFormat.OpenXml.Bibliography.Author;

namespace LeetKumer.Service;

public static class ExportInfo
{
    public static void ExportToCsv(string filePath)
    {
        var books = DataService.GetFullInfo<Book>();
        StringBuilder csvContent = new StringBuilder();

        csvContent.AppendLine("Title,AuthorFullName,ISBN,YearOfManufacture,GenreName");

        foreach (var book in books)
        {
            var yearOfManufacture = DateTime.SpecifyKind(book.YearOfManufacture, DateTimeKind.Utc);

            using (var dbContext = new MyDbContext())
            {
                Models.Author? author = dbContext.Authors.FirstOrDefault(a => book.AuthorId == a.Id);
                Models.Genre? genre = dbContext.Genres.FirstOrDefault(g => book.AuthorId == g.Id);

                csvContent.AppendLine(
                    $"{book.Title}," +
                    $"{author.FullName}," +
                    $"{book.ISBN}," +
                    $"{yearOfManufacture}," +
                    $"{genre.Name},");
            }

            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
        }
    }
    
    
    public static void ExportToExcel(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        var books = DataService.GetFullInfo<Book>();
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Books");
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "AuthorFullName";
            worksheet.Cells[1, 3].Value = "ISBN";
            worksheet.Cells[1, 4].Value = "YearOfManufacture";
            worksheet.Cells[1, 5].Value = "GenreName";

            for (int i = 0; i < books.Count; i++)
            {
                var yearOfManufacture = DateTime.SpecifyKind(books[i].YearOfManufacture, DateTimeKind.Utc);
                using (var dbContext = new MyDbContext())
                {
                    Models.Author? author = dbContext.Authors.FirstOrDefault(a => books[i].AuthorId == a.Id);
                    Models.Genre? genre = dbContext.Genres.FirstOrDefault(g => books[i].AuthorId == g.Id);

                    worksheet.Cells[i + 2, 1].Value = books[i].Title;
                    worksheet.Cells[i + 2, 2].Value = author.FullName;
                    worksheet.Cells[i + 2, 3].Value = books[i].ISBN;
                    worksheet.Cells[i + 2, 4].Value = yearOfManufacture;
                    worksheet.Cells[i + 2, 5].Value = genre.Name;
                }
            }

            package.SaveAs(new FileInfo(filePath));
        }
    }

    public static void ExportToPDF(string filePath)
    {
        var books = DataService.GetFullInfo<Book>();
        var document = new Document();
        PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        var font = FontFactory.GetFont(FontFactory.HELVETICA, 12);

        var table = new PdfPTable(5);
        table.AddCell(new Phrase("Title", font));
        table.AddCell(new Phrase("ISBN", font));
        table.AddCell(new Phrase("YearOfManufacture", font));
        table.AddCell(new Phrase("AuthorFullName", font));
        table.AddCell(new Phrase("GenreName", font));
        table.AddCell(new Phrase("Description", font));

        foreach (var book in books)
        {
            using (var dbContext = new MyDbContext())
            {
                Models.Author? author = dbContext.Authors.FirstOrDefault(a => book.AuthorId == a.Id);
                Models.Genre? genre = dbContext.Genres.FirstOrDefault(g => book.AuthorId == g.Id);


                table.AddCell(new Phrase(book.Title ?? "N/A", font)); // Проверка на null
                table.AddCell(new Phrase(book.ISBN ?? "N/A", font)); // Проверка на null
                table.AddCell(new Phrase(book.YearOfManufacture.ToString(), font));
                table.AddCell(new Phrase(author.FullName, font));
                table.AddCell(new Phrase(genre.Name, font));
                table.AddCell(new Phrase(book.Description ?? "N/A", font)); // Проверка на null
            }

            document.Add(table);
            document.Close();
        }
    }
}