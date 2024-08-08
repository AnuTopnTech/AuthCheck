using System.ComponentModel.DataAnnotations;

namespace AuthCheck.Server.Dtos
{
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
    }
}
