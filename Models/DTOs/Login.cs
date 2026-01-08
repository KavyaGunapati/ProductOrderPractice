using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class Login
    {
        [Required]
        public string Email { get; set; }=null!;
        [Required]
        public string Password { get; set; }=null!;
    }
}