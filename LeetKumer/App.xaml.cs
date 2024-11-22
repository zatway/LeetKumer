using System.Configuration;
using System.Data;
using System.Windows;
using LeetKumer.Migrations;
using LeetKumer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeetKumer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Вызов метода проверки и применения миграций
        CreateDBOrExistsCheck();
    }

    /// <summary>
    /// Проверка на существование базы данных, ее создание
    /// </summary>
    public static void CreateDBOrExistsCheck()
    {
        using (var dbContext = new MyDbContext())
        {
            if (!dbContext.Database.GetService<IRelationalDatabaseCreator>().Exists()) // проверка на существование бд
                dbContext.Database.EnsureCreated(); // создание бд
            ApplyMigrations(dbContext);
        }
    }

    /// <summary>
    /// Подключение миграций
    /// </summary>
    /// <param name="dbContext">Коннтекст базы данных</param>
    private static void ApplyMigrations(MyDbContext dbContext)
    {
        try
        {
            IMigrator migrator = new AddSearchAndFilterFunction_books();
            migrator.Migrate();
            dbContext.Database.GetMigrations();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Ошибка применения миграций: {ex.Message}");
        }
    }
}