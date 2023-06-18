using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Globalization;
using SportsSchemaBuilder.Services;

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FitFilesController : ControllerBase
    {
        //private readonly UserContext _context;
        //private readonly IConfiguration _configuration;
        private readonly IFitFileRepository _fitFileRepository;
        private readonly IAuthService _authService;

        public FitFilesController(IFitFileRepository fitFileRepository, IAuthService authService)
        {
            //_context = context;
            //_configuration = configuration;
            _fitFileRepository = fitFileRepository;
            _authService = authService;
        }




        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ActionName("Upload")]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            if (file.Length <= 0)
            {
                return BadRequest("Empty file");
            }

            if (file.FileName.Substring(file.FileName.Length - 3, 3) != "fit")
            {
                return BadRequest("Wrong Format");
            }


            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            //Console.WriteLine($"Received file {file.FileName} with size in bytes {file.Length}");


            //check if file exists already
            var UserHasFile = await _fitFileRepository.CheckIfFileExists(name, file);

            
            if(UserHasFile.FitFiles.Any() == false)
            {


            try{

               await _fitFileRepository.Add(file, UserHasFile.Id);


            }
                catch (Exception ex) {

                    return BadRequest(new { error = ex });
                }


                
            }


            return Ok(new { mes = "ja" });
        }

        [HttpGet]
        [Authorize]
        [ActionName("Fitfiles")]
        public IActionResult GetFitFiles()
        {
            string name = _authService.GetPayloadName(Request.Headers.Authorization);
            var result = _fitFileRepository.FitFilesList(name);
            
            return Ok(result);
        }


        [HttpGet("{id}", Name = "Download")]
        [Authorize]
        
        //[Produces("application/octet-stream")]
        public IActionResult Download(int id)
        {
            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            var requestedFitFile = _fitFileRepository.GetFitFileById(name, id);

            
            if (requestedFitFile.FitFiles.Any())
            {
                string path = requestedFitFile.FitFiles[0].Path;
                byte[] readBytes = System.IO.File.ReadAllBytes(path);

                return File(readBytes, "application/octet-stream", requestedFitFile.FitFiles[0].FileName);
                    

            }

            return BadRequest("File not found");
        }


        [HttpDelete("{id}", Name = "Delete")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {

            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            var UserHasFile = _fitFileRepository.GetFitFileById(name, id);

            if (UserHasFile.FitFiles.Any() == true)
            {

                if (System.IO.File.Exists(UserHasFile.FitFiles[0].Path))
                {
                    System.IO.File.Delete(UserHasFile.FitFiles[0].Path);
                }

                _fitFileRepository.Remove(UserHasFile.FitFiles[0]);
                await _fitFileRepository.SaveChangesAsync();

                return Ok(new {message = "file deleted" });
            }

           
            return BadRequest("Unable to delete file");

        }




    }
}
