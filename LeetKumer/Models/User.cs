using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeetKumer.Models;

public enum RoleEnum
{
        Admin = 1,
        User = 2
}

public class User
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public RoleEnum Role { get; set; }
    
}