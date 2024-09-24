using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Models
{
    public class UsernameCount
    {
        [Key]
        public int Year { get; set; } // The year for which the count is recorded
        public int Count { get; set; } // The number of usernames generated in that year
    }
}
