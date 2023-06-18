using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Controllers;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Dto;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SportsSchemaBuilder.Services
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly UserContext _context;
        public CalendarRepository(UserContext context)
        {
            _context = context;
        }
        public void AddWorkout(CalendarWorkout CalendarWorkout, int Id)
        {
            UserCalendarWorkout userCalendarWorkout = new UserCalendarWorkout();
            userCalendarWorkout.Category = CalendarWorkout.Category;
            userCalendarWorkout.DurationHours = CalendarWorkout.DurationHours;
            userCalendarWorkout.DurationMinutes = CalendarWorkout.DurationMinutes;
            userCalendarWorkout.Description = CalendarWorkout.Description;
            userCalendarWorkout.date = CalendarWorkout.date;
            userCalendarWorkout.UploadDate = DateTime.Now;
            userCalendarWorkout.UserId = Id;


            if (CalendarWorkout.exerciseList != null)
            {


                foreach (ExercisesTemplate exercise in CalendarWorkout.exerciseList)
                {
                    Exercises Exercise = new Exercises();
                    Exercise.exerciseName = exercise.exerciseName;
                    Exercise.sets = exercise.sets;
                    Exercise.reps = exercise.reps;
                    userCalendarWorkout.Exercises.Add(Exercise);

                }
            }

            _context.UserCalendar.Add(userCalendarWorkout);

        }
        public void UpdateWorkout(UserCalendarWorkout dbActivity, UpdateWorkout updateWorkout)
        {
            List<Exercises> exerciseList = new List<Exercises>();

            if (updateWorkout.DurationMinutes != null && updateWorkout.DurationMinutes < 60 && updateWorkout.DurationMinutes >= 0)
            {
                dbActivity.DurationMinutes = updateWorkout.DurationMinutes;
            }
            if (updateWorkout.DurationHours != null)
            {
                dbActivity.DurationHours = updateWorkout.DurationHours;
            }
            if (updateWorkout.Description != null)
            {
                dbActivity.Description = updateWorkout.Description;
            }



            foreach (Exercises Exercise in dbActivity.Exercises)
            {
                exerciseList.Add(Exercise);
                foreach (ExercisesTemplate exercise in updateWorkout.exerciseList)
                {
                    if (Exercise.Id == exercise.Id)
                    {
                        Exercise.exerciseName = exercise.exerciseName;
                        Exercise.sets = exercise.sets;
                        Exercise.reps = exercise.reps;

                    }


                }

                // deleting an exercise
                foreach (int deleteId in updateWorkout.deleteIdList)
                {
                    if (Exercise.Id == deleteId)
                    {
                        exerciseList.Remove(Exercise);

                    }
                }




            }

            foreach (ExercisesTemplate exercise in updateWorkout.exerciseList)
            {
                if (exercise.Id == null)
                {
                    Exercises newExercise = new Exercises();

                    newExercise.exerciseName = exercise.exerciseName;
                    newExercise.sets = exercise.sets;
                    newExercise.reps = exercise.reps;
                    newExercise.UserCalendarWorkout = dbActivity;
                    exerciseList.Add(newExercise);
                }
            }

            dbActivity.Exercises = exerciseList;
        }

        public List<User> GetActivitiesList(string name, DateTime firstDate, DateTime lastDate)
        {
            return _context.Users
                           .Where(u => u.Name == name).Include(c => c.CalendarActivities.OrderBy(e => e.date).Where(c => c.date >= firstDate && c.date <= lastDate)).ThenInclude(u => u.Exercises).ToList();
        }

        public UserCalendarWorkout GetActivity(int id, int dbUserId)
        {
            return _context.UserCalendar.Where(u => u.UserId == dbUserId && u.Id == id).Include(e => e.Exercises).FirstOrDefault();
            
        }

        public void Remove(UserCalendarWorkout dbActivity)
        {
            _context.UserCalendar.Remove(dbActivity);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }

    
}
