using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SportsSchemaBuilder.Models;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Diagnostics;

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        public CalendarController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("{firstDate}/{lastDate}", Name = "CalendarData")]
        [Authorize]
        [ActionName("CalendarData")]

        public IActionResult GetCalendarData(DateTime firstDate, DateTime lastDate)
        {


            int res = DateTime.Compare(firstDate, lastDate);

            if (res >= 0)
            {
                return BadRequest("Wrong Dates");
            }
            string name = GetPayloadName(Request.Headers.Authorization);

            try {
                var UserCalendarActivities = _context.Users
                           .Where(u => u.Name == name).Include(c => c.CalendarActivities.OrderBy(e => e.date).Where(c => c.date >= firstDate && c.date <= lastDate)).ThenInclude(u => u.Exercises).ToList();

                List<CalendarWorkout> result = new List<CalendarWorkout>();
                foreach (var Activity in UserCalendarActivities[0].CalendarActivities) {
                    CalendarWorkout calendarWorkout = new CalendarWorkout();
                    calendarWorkout.Id = Activity.Id;
                    calendarWorkout.Category = Activity.Category;
                    calendarWorkout.DurationMinutes = Activity.DurationMinutes;
                    calendarWorkout.DurationHours = Activity.DurationHours;
                    calendarWorkout.Description = Activity.Description;
                    calendarWorkout.date = Activity.date;
                    if (Activity.Exercises.Count > 0)
                    {
                        foreach (Exercises exercise in Activity.Exercises)
                        {
                            ExercisesTemplate Exercise = new ExercisesTemplate();
                            Exercise.Id = exercise.Id;
                            Exercise.exerciseName = exercise.exerciseName;
                            Exercise.sets = exercise.sets;
                            Exercise.reps = exercise.reps;
                            calendarWorkout.exerciseList.Add(Exercise);

                        }
                    }
                    result.Add(calendarWorkout);
                }


                return Ok(new { activities = result });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex });
            }





        }

        [HttpPost]
        [Authorize]
        [ActionName("Upload")]
        public async Task<ActionResult> Upload(CalendarWorkout CalendarWorkout)
        {
            string name = GetPayloadName(Request.Headers.Authorization);
            var DbUser = _context.Users
                        .SingleOrDefault(e => e.Name == name);
            if (DbUser != null) {
                try
                {
                    UserCalendarWorkout userCalendarWorkout = new UserCalendarWorkout();
                    userCalendarWorkout.Category = CalendarWorkout.Category;
                    userCalendarWorkout.DurationHours = CalendarWorkout.DurationHours;
                    userCalendarWorkout.DurationMinutes = CalendarWorkout.DurationMinutes;
                    userCalendarWorkout.Description = CalendarWorkout.Description;
                    userCalendarWorkout.date = CalendarWorkout.date;
                    userCalendarWorkout.UploadDate = DateTime.Now;
                    userCalendarWorkout.UserId = DbUser.Id;


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
                    await _context.SaveChangesAsync();

                    return Created("Upload", new { res = "Activity successfully uploaded"} );
                    }
                catch (Exception ex)
                {

                    return BadRequest(new { error = ex });
                }
            }

            return BadRequest(new { res = "Unable to upload activity" });
        }



   

        [HttpPut]
        [Authorize]
        [ActionName("UpdateActivity")]
        public async Task<ActionResult> Update(UpdateWorkout UpdateWorkout)
        {
            
            string name = GetPayloadName(Request.Headers.Authorization);
            
            var dbUser = _context.Users.Where(u => u.Name == name).FirstOrDefault();

            var dbActivity = _context.UserCalendar.Where(u => u.UserId == dbUser.Id && u.Id == UpdateWorkout.Id).Include(e => e.Exercises).FirstOrDefault();

            if(dbActivity != null)
            {

            try {
                    List<Exercises> exerciseList = new List<Exercises>();
                    
                    if (UpdateWorkout.DurationMinutes != null && UpdateWorkout.DurationMinutes < 60 && UpdateWorkout.DurationMinutes >= 0)
                    {
                        dbActivity.DurationMinutes = UpdateWorkout.DurationMinutes;
                    }
                    if (UpdateWorkout.DurationHours != null)
                    {
                        dbActivity.DurationHours = UpdateWorkout.DurationHours;
                    }
                    if (UpdateWorkout.Description != null)
                    {
                        dbActivity.Description = UpdateWorkout.Description;
                    }
                    


                    foreach (Exercises Exercise in dbActivity.Exercises)
                        {
                        exerciseList.Add(Exercise);
                            foreach (ExercisesTemplate exercise in UpdateWorkout.exerciseList)
                            {
                                if (Exercise.Id == exercise.Id)
                                {
                                Exercise.exerciseName = exercise.exerciseName;
                                Exercise.sets = exercise.sets;
                                Exercise.reps = exercise.reps;
                                
                            }
                                

                        }

                        // deleting an exercise
                        foreach (int deleteId in UpdateWorkout.deleteIdList)
                        {
                            if (Exercise.Id == deleteId)
                            {
                                exerciseList.Remove(Exercise);

                            }
                        }




                    }

                    foreach (ExercisesTemplate exercise in UpdateWorkout.exerciseList)
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


                    
                    await _context.SaveChangesAsync();
                    return NoContent();
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex });
            }
            }
            return BadRequest("Not able to update");
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ActionName("DeleteActivity")]
        public async Task<ActionResult> Delete(int id)
        {

            string name = GetPayloadName(Request.Headers.Authorization);
            //string name = "raf";
            var dbUser = _context.Users.Where(u => u.Name == name).FirstOrDefault();
            var dbActivity = _context.UserCalendar.Where(u => u.UserId == dbUser.Id && u.Id == id).Include(e => e.Exercises).FirstOrDefault();

            if (dbActivity != null)
            {
                
                _context.UserCalendar.Remove(dbActivity);
                await _context.SaveChangesAsync();
                return NoContent();

            }

            return BadRequest(new { res = "Unable to delete activity" });
        }
        private string GetPayloadName(string authorization)
        {
            string tokenstring = authorization;
            tokenstring = tokenstring.Remove(0, 7);

            var jwt = new JwtSecurityToken(jwtEncodedString: tokenstring);
            string name = jwt.Claims.First(c => c.Type == "name").Value;

            return name;
        }
    }

    
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

    public class UpdateWorkout
    {
        public int Id { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }

        public string? Description { get; set; }

        public List<ExercisesTemplate>? exerciseList { get; set; } = new List<ExercisesTemplate>();

        public  List<int>? deleteIdList { get; set; } = new List<int>();

    }

    public class ExercisesTemplate
    {
        public int? Id { get; set; }
        public string exerciseName { get; set; }

        public int sets { get; set; }
        public int reps { get; set; }
    }
}
