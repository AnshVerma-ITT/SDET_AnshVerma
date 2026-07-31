namespace GourmetSpot.Models
{
    public abstract class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public bool IsFinalized { get; set; }
        public virtual int TableNumber { get; protected set; }
        public abstract string OrderType { get; }
        public virtual bool CanReceiveSubOrder => false;

        private readonly List<SubOrder> subOrders;
        private readonly List<OrderItem> items;

        public IReadOnlyList<SubOrder> SubOrders => subOrders;
        public IReadOnlyList<OrderItem> Items => items;

        protected Order(
            int orderId,
            string customerName = "Walk-in Customer",
            bool isFinalized = true)
        {
            OrderId = orderId;
            CustomerName = customerName;
            IsFinalized = isFinalized;
            TableNumber = 0;
            subOrders = new List<SubOrder>();
            items = new List<OrderItem>();
        }

        public void AddSubOrder(SubOrder subOrder)
        {
            subOrders.Add(subOrder);
            foreach (OrderItem orderItem in subOrder.Items)
            {
                items.Add(orderItem);
            }
        }

        public void RebuildItemsFromSubOrders()
        {
            items.Clear();
            foreach (SubOrder subOrder in subOrders)
            {
                foreach (OrderItem orderItem in subOrder.Items)
                {
                    items.Add(orderItem);
                }
            }
        }
    }
}
