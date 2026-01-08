using Microsoft.AspNetCore.Identity;
namespace DataAccess.Entities
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }=null!;
    }
}