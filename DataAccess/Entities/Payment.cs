namespace DataAccess.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; }=null!;
        public int OrderId { get; set; }
        public Order Order { get; set; }=null!;
        public string Status { get; set; }=null!;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}