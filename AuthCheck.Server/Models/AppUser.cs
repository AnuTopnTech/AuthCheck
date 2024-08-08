using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCheck.Server.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? ProfileImage { get; set; }
        public  string ? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
