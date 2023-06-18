using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using System.Collections.Generic;
using System.Globalization;

namespace SportsSchemaBuilder.Services
{
    public class FitFileRepository : IFitFileRepository
    {

        private readonly UserContext _context;
        public FitFileRepository(UserContext context)
        {
            _context = context;
        }

        public async Task Add(IFormFile file, int id)
        {
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
            fitFile.UserId = id;

            _context.UserFitFiles.Add(fitFile);
            await SaveChangesAsync();

            //Save the file to folder
            using (var stream = System.IO.File.Create(uniqueFilePath))
            {
                await file.CopyToAsync(stream);
            }
        }
        public User GetFitFileById(string name, int id)
        {
            return _context.Users.Where(u => u.Name == name).Include(u => u.FitFiles.Where(o => o.Id == id)).FirstOrDefault();
        }

        public async Task<User> CheckIfFileExists(string name, IFormFile file)
        {
            return _context.Users.Where(u => u.Name == name).Include(u => u.FitFiles.Where(o => o.FileName == file.FileName && o.BytesLength == file.Length)).FirstOrDefault();
            
        }


        public void Remove(UserFitFiles file)
        {
            _context.UserFitFiles.Remove(file);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public List<Object> FitFilesList(string name)
        {
            var UserFitFilesList = _context.Users.Where(u => u.Name == name).Include(u => u.FitFiles.OrderByDescending(e => e.date)).ToList();

            List<object> result = new List<object>();
            foreach (var u in UserFitFilesList[0].FitFiles)
            {
                result.Add(new { id = u.Id, title = u.Title, filename = u.FileName });
            }

            return result;
        }
    }
}
