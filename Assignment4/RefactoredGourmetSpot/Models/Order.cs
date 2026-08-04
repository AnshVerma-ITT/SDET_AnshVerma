namespace GourmetSpot.Models
{
    public abstract class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public bool IsFinalized { get; set; }
        public int TableNumber { get; protected set; }
        public OrderType OrderType { get; protected set; }
        public List<SubOrder> SubOrders { get; set; }
        public List<OrderItem> Items { get; set; }

        protected Order(
            int orderId,
            string customerName = "Walk-in Customer",
            bool isFinalized = true,
            OrderType orderType = OrderType.Takeaway)
        {
            OrderId = orderId;
            CustomerName = customerName;
            IsFinalized = isFinalized;
            OrderType = orderType;
            TableNumber = 0;
            SubOrders = new List<SubOrder>();
            Items = new List<OrderItem>();
        }

        // Models hold order data. Order behavior stays in services.
    }
}
