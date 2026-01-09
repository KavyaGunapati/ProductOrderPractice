using System.ComponentModel.DataAnnotations;
namespace Models.DTOs
{
    public class Register
    {
        [Required]
        public string FullName { get; set; }=null!;
        [Required]
        public string Email { get; set; }=null!;
        public string Password { get; set; }=null!;
        public string Roles { get; set; }= "User";
    }
}