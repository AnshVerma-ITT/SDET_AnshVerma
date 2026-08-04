namespace GourmetSpot.Models
{
    public class SubOrder
    {
        public int SubOrderNumber { get; set; }
        public DateTime OrderedAt { get; set; }
        public List<OrderItem> Items { get; set; }

        public SubOrder(int subOrderNumber, DateTime orderedAt)
        {
            SubOrderNumber = subOrderNumber;
            OrderedAt = orderedAt;
            Items = new List<OrderItem>();
        }
    }
}
