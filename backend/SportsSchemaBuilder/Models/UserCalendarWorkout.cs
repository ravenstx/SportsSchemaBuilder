using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SportsSchemaBuilder.Models
{
    public class UserCalendarWorkout
    {
        [Key]
        public int Id { get; set; }


        public int Category { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }

        public string? Description { get; set; }

        public List<Exercises>? Exercises { get; set; } = new List<Exercises>();
        public DateTime date { get; set; }

        public DateTime UploadDate { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }


        
    }
}