using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SportsSchemaBuilder.Models
{
    public class UserFitFiles
    {
        [Key]
        public int Id { get; set; }


        public string Title { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public long BytesLength { get; set; }

        public DateTime date { get; set; }
        public DateTime UploadDate { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }


    }
}
