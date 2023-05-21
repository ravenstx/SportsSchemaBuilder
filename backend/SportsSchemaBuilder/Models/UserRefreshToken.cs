using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsSchemaBuilder.Models
{
    public class UserRefreshToken
    {
        [Key]
        public int TokenId { get; set; }
        public string Token { get; set; }

        public DateTime TokenExpiration { get; set; }


        public User User { get; set; }
        public int UserId { get; set; } 


    }
}
