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


        //Razorpay Payment Details
        public int PaymentId { get; set; }//set after payment is done
        public Payment Payment { get; set; }=null!;//set after payment is done
        public string PaymentStatus { get; set; }="Pending";//Pending/Authorized/Captured/Failed
    }
}