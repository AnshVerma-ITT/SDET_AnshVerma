using System.Collections.Generic;

namespace GourmetSpot.Models
{
    public class Order
    {
        private const decimal TaxRate = 0.18m;
        public int OrderId { get; set; }
        public List<OrderItem> Items { get; set; }
        public Order(int orderId)
        {
            OrderId = orderId;
            Items = new List<OrderItem>();
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public decimal CalculateSubtotal()
        {
            decimal subtotal = 0;
            foreach (OrderItem item in Items)
            {
                subtotal += item.TotalPrice;
            }
            return subtotal;
        }
        public decimal CalculateTax()
        {
            return CalculateSubtotal() * TaxRate;
        }
        public decimal CalculateGrandTotal()
        {
            return CalculateSubtotal() +CalculateTax();
        }
    }
}