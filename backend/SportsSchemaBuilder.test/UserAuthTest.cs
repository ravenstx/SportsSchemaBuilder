using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SportsSchemaBuilder;
using SportsSchemaBuilder.Controllers;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Dto;
using SportsSchemaBuilder.Models;
using Azure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Web.Http.Results;
using NuGet.Protocol;
using Newtonsoft.Json;
using AutoMapper;
using SportsSchemaBuilder.Services;
using Moq;

namespace SportsSchemaBuilder.test
{
    [TestClass]
    public class UserAuthTest
    {

        private readonly IUserRepository _UserRepository;
        private readonly IAuthService _authService;
        private readonly UserAuthController _controller;



        public UserAuthTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _authService = new AuthService(config);
            _UserRepository = new UserRepositoryFake(_authService);
            _controller = new UserAuthController(_UserRepository, _authService);
        }

        [TestMethod]
        public async Task TestRegister()
        {
            var user = new UserTemplate();
            user.Name = "test";
            user.Password = "pass";

            var actionResult = await _controller.Register(user);

            //Console.WriteLine(result.StatusCode);
            Assert.AreEqual(actionResult.GetType(), typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task TestLogin()
        {

            // Arrange

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };


            var user = new UserTemplate();
            user.Name = "test";
            user.Password = "pass";
            // Act

            var actionResult = await _controller.Login(user);
            // Assert


            var result = actionResult as OkObjectResult;
            //Console.WriteLine(result.Value.ToString());
            dynamic jsonResult = JsonConvert.DeserializeObject<object>(result.Value.ToJson());
            var token = jsonResult["Token"];
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // GET request user with jwt token

            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            //Act
            var actionResult2 = await _controller.GetUsers();

            //Arrange
            var result2 = actionResult2 as OkObjectResult;
            dynamic jsonResult2 = JsonConvert.DeserializeObject<object>(result2.Value.ToJson());
            //string name = jsonResult2["name"];
            Assert.IsNotNull(result2);
            Assert.AreEqual(user.Name, (string)jsonResult2["name"]);

        }




    }
}