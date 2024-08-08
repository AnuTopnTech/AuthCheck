using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCheck.Server.Dtos
{
    public class RegisterDtos
    {
        [Required]
        [EmailAddress]

        
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        //public List<string>? Roles { get; set; }

        ////[NotMapped]
        public string? Image { get; set; }
    }
}
