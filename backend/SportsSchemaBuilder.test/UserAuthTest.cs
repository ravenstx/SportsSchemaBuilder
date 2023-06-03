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


namespace SportsSchemaBuilder.test
{
    [TestClass]
    public class UserAuthTest
    {

        private readonly IConfiguration _configuration;
        


        [TestMethod]
        public async Task TestRegister()
        {
            var user = new UserAuthController.UserTemplate();
            user.Name = "test";
            user.Password = "pass";
            var dbContext = GetDatabaseContext();

            //act
            var controller = new UserAuthController(dbContext, _configuration);
            var actionResult = await controller.Register(user);

            //Console.WriteLine(result.StatusCode);
            Assert.AreEqual(actionResult.GetType(), typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task TestLogin()
        {

            // Arrange
            var dbContext = GetDatabaseContext();
            IConfiguration configuration = GetConfiguration();
            var controller = new UserAuthController(dbContext, configuration);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var user = new UserAuthController.UserTemplate();
            user.Name = "test";
            user.Password = "pass";
            // Act

            var actionResult = await controller.Login(user);
            // Assert
            

            var result = actionResult as OkObjectResult;
            //Console.WriteLine(result.Value.ToString());
            dynamic jsonResult = JsonConvert.DeserializeObject<object>(result.Value.ToJson());
            var token = jsonResult["Token"];
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // GET request user with jwt token

            // Arrange
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            //Act
            var actionResult2 = await controller.GetUsers();

            //Arrange
            var result2 = actionResult2 as OkObjectResult;
            dynamic jsonResult2 = JsonConvert.DeserializeObject<object>(result2.Value.ToJson());
            //string name = jsonResult2["name"];
            Assert.IsNotNull(result2);
            Assert.AreEqual(user.Name, (string)jsonResult2["name"]);

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