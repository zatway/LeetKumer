using LeetKumer.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace LeetKumer.Service
{
    public class MyDbContext : DbContext
    {
        //Таблицы в бд
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<CoverImage> CoverImages { get; set; }

       /// <summary>
       /// Подключение к БД
       /// </summary>
       /// <param name="optionsBuilder">строка подключения</param>       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LeetKumer;Username=postgres;Password=123;");
        }

        /// <summary>
        /// Создание таблиц в БД
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(255);
            });
            
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ISBN).HasMaxLength(25);
                entity.Property(e => e.YearOfManufacture).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.HasOne(e => e.CoverImage)
                    .WithMany(c => c.Books)
                    .HasForeignKey(e => e.CoverImageId)
                    .IsRequired(false); // Указываем, что связь необязательная
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("authors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genres");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<CoverImage>(entity =>
            {
                entity.ToTable("coverimages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageData).IsRequired();
            });

            modelBuilder.Entity<Book>()
                .HasOne(e => e.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(e => e.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(e => e.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(e => e.GenreId);

            modelBuilder.Entity<Book>()
                .HasOne(e => e.CoverImage)
                .WithMany(c => c.Books)
                .HasForeignKey(e => e.CoverImageId);
        }
    }
}
