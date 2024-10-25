using System.ComponentModel.DataAnnotations;

namespace TennisCoach.Models
{
    public class Admins

    {
        [Key]
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Password  { get; set; }
    }
}
