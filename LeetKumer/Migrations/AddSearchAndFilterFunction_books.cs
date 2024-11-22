using LeetKumer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeetKumer.Migrations
{
    internal class AddSearchAndFilterFunction_books : IMigrator
    {
        private readonly string _filterBooksSql = @"
        CREATE OR REPLACE FUNCTION public.filter_books(
        search_query TEXT,
        filter_column TEXT,
        page_number INT,
        page_size INT
        )
        RETURNS TABLE(
            ""Id"" INT,
            ""Title"" VARCHAR(70),
            ""AuthorId"" INT,
            ""YearOfManufacture"" TIMESTAMP WITH TIME ZONE,
            ""ISBN"" VARCHAR(30),
            ""GenreId"" INT,
            ""CoverImageId"" INT,
            ""Description"" VARCHAR(100) -- Изменение типа данных здесь
        )
        LANGUAGE 'plpgsql'
        AS $BODY$
        BEGIN
            RETURN QUERY
            SELECT
                b.""Id"",
                b.""Title"",
                b.""AuthorId"",
                b.""YearOfManufacture"",
                b.""ISBN"",
                b.""GenreId"",
                b.""CoverImageId"",
                b.""Description""
            FROM public.books b
            LEFT JOIN public.authors a ON b.""AuthorId"" = a.""Id""
            LEFT JOIN public.genres g ON b.""GenreId"" = g.""Id""
            WHERE
                (search_query IS NULL OR
                 search_query = '' OR
                 b.""Title"" ILIKE '%' || search_query || '%')
            ORDER BY
                CASE WHEN filter_column = 'По названию' THEN b.""Title""
                     WHEN filter_column = 'По автору' THEN a.""FullName""
                     WHEN filter_column = 'По жанру' THEN g.""Name""
                     WHEN filter_column = 'По году выпуска' THEN b.""YearOfManufacture""::TEXT
                     ELSE b.""Id""::TEXT -- Default sorting by id
                END
            OFFSET (page_number - 1) * page_size
            LIMIT page_size;
        END;
        $BODY$;
        ";

        public void Migrate(string? targetMigration = null)
        {
            using (var dbContext = new MyDbContext())
            {
                dbContext.Database.ExecuteSqlRaw(_filterBooksSql);
            }
        }

        public async Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default)
        {
            using (var dbContext = new MyDbContext())
            {
                await dbContext.Database.ExecuteSqlRawAsync(_filterBooksSql, cancellationToken);
            }
        }

        public string GenerateScript(string? fromMigration = null, string? toMigration = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        {
            return _filterBooksSql;
        }
        
        public bool HasPendingModelChanges()
        {
            // This can be implemented as needed, for now, we can return false
            return false; // For now, we assume no pending changes to the model
        }
    }
}
