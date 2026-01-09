namespace Models.DTOs
{
    public class TokenData
    {
        public string UserId { get; set; }=null!;
        public string Email { get; set; }=null!;
        public string Token { get; set; }=null!;
        public IList<string> Roles { get; set; }=new List<string>();
    }
}