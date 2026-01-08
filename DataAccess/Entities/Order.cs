namespace DataAccess.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string AppUserId { get; set; }=null!;
        public AppUser AppUser { get; set; }=null!;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; }=new List<OrderItem>();
        public int Quantity { get; set; }
    }
}