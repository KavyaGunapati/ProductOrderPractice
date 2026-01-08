namespace DataAccess.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }=null!;
        public List<Product> Products { get; set; }=new List<Product>();
    }
}