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
using SportsSchemaBuilder.Services;
using SportsSchemaBuilder.Dto;
using AutoMapper;
using System;

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        //private readonly UserContext _context;
        //private readonly IConfiguration _configuration;
        private readonly ICalendarRepository _CalendarRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _Mapper;
        public CalendarController(ICalendarRepository calendarRepository, IUserRepository userRepository, IAuthService authService, IMapper mapper)
        {
            //_context = context;
            //_configuration = configuration;
            _CalendarRepository = calendarRepository;
            _UserRepository = userRepository;
            _authService = authService;
            _Mapper = mapper;
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
            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            try {
                
                var UserCalendarActivities = _CalendarRepository.GetActivitiesList(name, firstDate, lastDate);

                var calendarWorkoutlist = UserCalendarActivities[0].CalendarActivities.Select(activity => _Mapper.Map<CalendarWorkout>(activity));

                return Ok(new { activities = calendarWorkoutlist });    
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
            string name = _authService.GetPayloadName(Request.Headers.Authorization);
            var DbUser = await _UserRepository.GetUser(name);
            if (DbUser != null) {
                try
                {
                    _CalendarRepository.AddWorkout(CalendarWorkout, DbUser.Id);
                    await _CalendarRepository.SaveChangesAsync();

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
            
            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            var DbUser = await _UserRepository.GetUser(name);

            var dbActivity = _CalendarRepository.GetActivity(UpdateWorkout.Id, DbUser.Id);

            if (dbActivity != null)
            {

            try {

                    _CalendarRepository.UpdateWorkout(dbActivity, UpdateWorkout);
                    
                    await _CalendarRepository.SaveChangesAsync();
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

            string name = _authService.GetPayloadName(Request.Headers.Authorization);
            //string name = "raf";
            var dbUser = await _UserRepository.GetUser(name);
            var dbActivity = _CalendarRepository.GetActivity(id, dbUser.Id);
            

            if (dbActivity != null)
            {
                _CalendarRepository.Remove(dbActivity);
                await _CalendarRepository.SaveChangesAsync();
                return NoContent();

            }

            return BadRequest(new { res = "Unable to delete activity" });
        }
       
    }

   


    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<UserCalendarWorkout, CalendarWorkout>()
                .ForMember(s => s.exerciseList, c => c.MapFrom(m => m.Exercises));
                
            CreateMap<Exercises, ExercisesTemplate>();

        }  
    }
}
