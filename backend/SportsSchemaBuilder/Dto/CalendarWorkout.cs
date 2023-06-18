using SportsSchemaBuilder.Controllers;

namespace SportsSchemaBuilder.Dto
{
    public class CalendarWorkout
    {
        public int? Id { get; set; }


        public int Category { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }

        public string? Description { get; set; }

        public List<ExercisesTemplate>? exerciseList { get; set; } = new List<ExercisesTemplate>();

        public DateTime date { get; set; }
    }
}
