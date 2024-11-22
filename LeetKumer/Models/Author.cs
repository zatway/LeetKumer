using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeetKumer.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // SERIAL ID
        public int Id { get; set; }
        public string FullName { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
