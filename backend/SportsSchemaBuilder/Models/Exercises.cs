using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SportsSchemaBuilder.Models
{
    public class Exercises
    {
        [Key]
        public int Id { get; set; }
        public string exerciseName { get; set; }

        public int sets { get; set;}
        public int reps { get; set;}

        public  UserCalendarWorkout UserCalendarWorkout { get; set; }
}
}
