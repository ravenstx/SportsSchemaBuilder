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

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FitFilesController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        public FitFilesController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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


            string name = GetPayloadName(Request.Headers.Authorization);

            //Console.WriteLine($"Received file {file.FileName} with size in bytes {file.Length}");


            //check if file exists already
            var UserHasFile = _context.Users
                       .Where(u => u.Name == name).Include(u => u.FitFiles.Where(o => o.FileName == file.FileName && o.BytesLength == file.Length)).FirstOrDefault();

            
            if(UserHasFile.FitFiles.Any() == false)
            {


            try{

                

                DateTime fitFileDate = DateTime.ParseExact(file.FileName.Substring(0, 19), "yyyy-MM-dd-HH-mm-ss",
                                       System.Globalization.CultureInfo.InvariantCulture);


                //Create a unique file path
                var uniqueFileName = Path.GetRandomFileName();
                var uniqueFilePath = Path.Combine(@".\", "FitFiles", $"{uniqueFileName}.fit");


                UserFitFiles fitFile = new UserFitFiles();

                fitFile.Title = fitFileDate.ToString("m", CultureInfo.GetCultureInfo("en-US"));
                fitFile.FileName = file.FileName;
                fitFile.Path = uniqueFilePath;
                fitFile.BytesLength = file.Length;
                fitFile.date = fitFileDate;
                fitFile.UploadDate = DateTime.Now;
                fitFile.UserId = UserHasFile.Id;

                _context.UserFitFiles.Add(fitFile);
                await _context.SaveChangesAsync();

                //Save the file to folder
                using (var stream = System.IO.File.Create(uniqueFilePath))
                {
                    await file.CopyToAsync(stream);
                }

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
            string name = GetPayloadName(Request.Headers.Authorization);
            var UserFitFilesList = _context.Users
                       .Where(u => u.Name == name).Include(u => u.FitFiles.OrderByDescending(e => e.date)).ToList();

            List<object> result = new List<object>();
            foreach(var u in UserFitFilesList[0].FitFiles) {
                result.Add(new {id = u.Id ,title = u.Title ,filename = u.FileName });
            }
            
            return Ok(result);
        }


        [HttpGet("{id}", Name = "Download")]
        [Authorize]
        
        //[Produces("application/octet-stream")]
        public IActionResult Download(int id)
        {
            string name = GetPayloadName(Request.Headers.Authorization);

            var requestedFitFile = _context.Users
                       .Where(u => u.Name == name).Include(u => u.FitFiles.Where(o => o.Id == id)).FirstOrDefault();

            
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

            string name = GetPayloadName(Request.Headers.Authorization);

            var UserHasFile = _context.Users
                       .Where(u => u.Name == name).Include(u => u.FitFiles.Where(o => o.Id == id)).FirstOrDefault();

            if (UserHasFile.FitFiles.Any() == true)
            {

                if (System.IO.File.Exists(UserHasFile.FitFiles[0].Path))
                {
                    System.IO.File.Delete(UserHasFile.FitFiles[0].Path);
                }
                
                _context.UserFitFiles.Remove(UserHasFile.FitFiles[0]);
                await _context.SaveChangesAsync();

                return Ok(new {message = "file deleted" });
            }

            // wijzig dit nog
            return BadRequest("Unable to delete file");

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
}
