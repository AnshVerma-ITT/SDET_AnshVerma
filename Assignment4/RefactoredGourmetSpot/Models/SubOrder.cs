namespace GourmetSpot.Models
{
    public class SubOrder
    {
        public int SubOrderNumber { get; set; }
        public DateTime OrderedAt { get; set; }
        private readonly List<OrderItem> items;
        public IReadOnlyList<OrderItem> Items => items;

        public SubOrder(int subOrderNumber, DateTime orderedAt)
        {
            SubOrderNumber = subOrderNumber;
            OrderedAt = orderedAt;
            items = new List<OrderItem>();
        }

        public void AddItem(OrderItem orderItem)
        {
            items.Add(orderItem);
        }
    }
}
