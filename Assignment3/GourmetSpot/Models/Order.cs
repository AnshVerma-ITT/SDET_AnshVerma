using System.Collections.Generic;

namespace GourmetSpot.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> Items { get; set; }

        public Order(int orderId, string customerName = "Walk-in Customer")
        {
            OrderId = orderId;
            CustomerName = customerName;
            Items = new List<OrderItem>();
        }
    }
}
