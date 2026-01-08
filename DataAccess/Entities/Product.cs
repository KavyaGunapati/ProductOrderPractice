namespace DataAccess.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }=null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }=null!;
        public List<OrderItem> OrderItems { get; set; }=new List<OrderItem>();
    }
}