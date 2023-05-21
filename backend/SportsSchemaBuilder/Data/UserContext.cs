using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Reflection.Emit;

namespace SportsSchemaBuilder.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        public DbSet<UserFitFiles> UserFitFiles { get; set; }

        public DbSet<UserCalendarWorkout> UserCalendar { get; set; }


    }
}
