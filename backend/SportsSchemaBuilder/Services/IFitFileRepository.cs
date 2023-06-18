using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Models;

namespace SportsSchemaBuilder.Services
{
    public interface IFitFileRepository
    {
        User GetFitFileById(string name, int id);
        Task<User> CheckIfFileExists(string name, IFormFile file);
        Task Add(IFormFile file, int id);
        Task SaveChangesAsync();

        void Remove(UserFitFiles file);

        List<Object> FitFilesList(string name);
    }
}
