using System.ComponentModel.DataAnnotations;

namespace PROG6212.POE.ST10153536.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}
