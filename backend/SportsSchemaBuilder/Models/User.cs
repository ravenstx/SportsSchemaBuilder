using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportsSchemaBuilder.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string? Name { get; set; }


        [MinLength(4)]
        public string? HashedPassword { get; set; }

        
        public virtual List<UserRefreshToken>? refreshTokens { get; set; }

        public virtual List<UserFitFiles>? FitFiles { get; set; }

       
        public virtual List<UserCalendarWorkout>? CalendarActivities { get; set; }


    }
}
