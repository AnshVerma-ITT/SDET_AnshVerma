namespace GourmetSpot.Models
{
    public class Bill
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsSaved { get; set; }
        public string? SavedFilePath { get; set; }
        public string SaveMessage { get; set; }

        public Bill(
            int orderId,
            string customerName,
            List<OrderItem> items,
            decimal subtotal,
            decimal tax,
            decimal grandTotal,
            bool isSaved = false,
            string? savedFilePath = null,
            string saveMessage = "")
        {
            OrderId = orderId;
            CustomerName = customerName;
            Items = items;
            Subtotal = subtotal;
            Tax = tax;
            GrandTotal = grandTotal;
            IsSaved = isSaved;
            SavedFilePath = savedFilePath;
            SaveMessage = saveMessage;
        }
    }
}
