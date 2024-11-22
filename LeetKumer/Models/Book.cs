using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeetKumer.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // SERIAL ID
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime YearOfManufacture { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int? CoverImageId { get; set; } // Теперь это поле может быть null
        public CoverImage CoverImage { get; set; }
        public string Description { get; set; }
    }
}
