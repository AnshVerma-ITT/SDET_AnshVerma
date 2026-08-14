namespace GourmetSpot.Models
{
    public class TableOrder : Order
    {
        public TableOrder(
            int orderId,
            string customerName,
            int tableNumber,
            bool isFinalized = false)
            : base(orderId, customerName, isFinalized, OrderType.Table)
        {
            TableNumber = tableNumber;
        }
    }
}
