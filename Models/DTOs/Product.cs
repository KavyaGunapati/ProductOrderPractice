namespace Models.DTOs
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }=null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}