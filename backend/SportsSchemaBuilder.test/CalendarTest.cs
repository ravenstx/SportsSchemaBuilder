using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SportsSchemaBuilder;
using SportsSchemaBuilder.Controllers;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Dto;
using Azure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Web.Http.Results;
using NuGet.Protocol;
using Newtonsoft.Json;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using SportsSchemaBuilder.Services;

namespace SportsSchemaBuilder.test
{
    [TestClass]
    public class CalendarTest
    {
        private readonly ICalendarRepository _CalendarRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IAuthService _authService;
        private readonly UserAuthController _userController;
        private readonly CalendarController _calendarController;
        private readonly IMapper _Mapper;

        public CalendarTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _authService = new AuthService(config);
            _UserRepository = new UserRepositoryFake(_authService);
            _CalendarRepository = new CalendarRepositoryFake(_authService);
            _userController = new UserAuthController(_UserRepository, _authService);
            _calendarController = new CalendarController(_CalendarRepository, _UserRepository, _authService, _Mapper);
        }

        [TestMethod]
        public async Task TestUploadWorkout()
        {

            //Arrange
            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var user = new UserTemplate();
            user.Name = "test";
            user.Password = "pass";
            //Act

           var actionResult = await _userController.Login(user);
            //Assert


            var result = actionResult as OkObjectResult;
            dynamic jsonResult = JsonConvert.DeserializeObject<object>(result.Value.ToJson());
            var token = jsonResult["Token"];
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // POST request user with jwt token

            //Arrange
            _calendarController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            _calendarController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            var workout = new CalendarWorkout();
            workout.Category = 0;
            workout.DurationHours = 2;
            workout.DurationMinutes = 30;
            workout.Description = "zone 2";

            //Act
            var calendarActionResult = await _calendarController.Upload(workout);

            //Arrange
            Assert.IsNotNull(calendarActionResult);
            Assert.AreEqual(_CalendarRepository.countActivities(), 1);

        }



    }
}