using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Controllers;
using SportsSchemaBuilder.Dto;

namespace SportsSchemaBuilder.Services
{
    public interface ICalendarRepository
    {
        void AddWorkout(CalendarWorkout CalendarWorkout, int Id);

        void UpdateWorkout(UserCalendarWorkout dbActivity, UpdateWorkout updateWorkout);

        List<User> GetActivitiesList(string name, DateTime firstDate, DateTime lastDate);

        int countActivities();

        UserCalendarWorkout GetActivity(int id, int dbUserId);

        void Remove(UserCalendarWorkout dbActivity);
        bool Save();
        Task SaveChangesAsync();
    }
}
