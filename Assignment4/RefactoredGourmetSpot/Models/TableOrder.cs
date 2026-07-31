namespace GourmetSpot.Models
{
    public class TableOrder : Order
    {
        public override string OrderType => OrderTypes.Table;
        public override bool CanReceiveSubOrder => !IsFinalized;

        public TableOrder(
            int orderId,
            string customerName,
            int tableNumber,
            bool isFinalized = false)
            : base(orderId, customerName, isFinalized)
        {
            TableNumber = tableNumber;
        }
    }
}
