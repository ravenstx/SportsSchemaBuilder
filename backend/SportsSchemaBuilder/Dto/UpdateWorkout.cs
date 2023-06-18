namespace SportsSchemaBuilder.Dto
{
    public class UpdateWorkout
    {
        public int Id { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }

        public string? Description { get; set; }

        public List<ExercisesTemplate>? exerciseList { get; set; } = new List<ExercisesTemplate>();

        public List<int>? deleteIdList { get; set; } = new List<int>();
    }
}
