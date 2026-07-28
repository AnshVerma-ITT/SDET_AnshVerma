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

        public void AddItem(OrderItem orderItem)
        {
            Items.Add(orderItem);
        }

        public decimal CalculateSubtotal()
        {
            decimal subtotal = 0;
            foreach (OrderItem orderItem in Items)
            {
                subtotal += orderItem.TotalPrice;
            }

            return subtotal;
        }

    }
}
