using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SportsSchemaBuilder;
using SportsSchemaBuilder.Controllers;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using Azure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Web.Http.Results;
using NuGet.Protocol;
using Newtonsoft.Json;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SportsSchemaBuilder.test
{
    [TestClass]
    public class CalendarTest
    {

        [TestMethod]
        public async Task TestUploadWorkout()
        {

            // Arrange
            var dbContext = GetDatabaseContext();
            IConfiguration configuration = GetConfiguration();
            var Usercontroller = new UserAuthController(dbContext, configuration);
            Usercontroller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var user = new UserAuthController.UserTemplate();
            user.Name = "test";
            user.Password = "pass";
            // Act

            var actionResult = await Usercontroller.Login(user);
            // Assert


            var result = actionResult as OkObjectResult;
            dynamic jsonResult = JsonConvert.DeserializeObject<object>(result.Value.ToJson());
            var token = jsonResult["Token"];
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // POST request user with jwt token

            // Arrange
            var controller = new CalendarController(dbContext, configuration);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            var workout = new CalendarWorkout();
            workout.Category = 0;
            workout.DurationHours = 2;
            workout.DurationMinutes = 30;
            workout.Description = "zone 2";

            //Act
            var calendarActionResult = await controller.Upload(workout);

            //Arrange
            Assert.IsNotNull(calendarActionResult);
            Assert.AreEqual(dbContext.UserCalendar.Count(), 1);

        }



            public UserContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new UserContext(options);
            databaseContext.Database.EnsureCreated();

            for (int i = 1; i <= 1; i++)
            {
                databaseContext.Users.Add(new User()
                {
                    Id = i,
                    Name = "test",
                    HashedPassword = UserAuthController.HashPassword("pass")

                });
                databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        public IConfiguration GetConfiguration()
        {

            var myConfiguration = new Dictionary<string, string>
{
    {"Jwt:Key", "t6zYbHDE4l2ypCeAS6Pg"},
    {"Jwt:Issuer", "http://localhost:5053"},
    {"Jwt:Audience", "http://localhost:5053"}
};

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }

    }
}