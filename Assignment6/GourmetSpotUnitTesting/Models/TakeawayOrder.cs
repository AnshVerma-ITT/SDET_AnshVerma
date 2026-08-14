namespace GourmetSpot.Models
{
    public class TakeawayOrder : Order
    {
        public TakeawayOrder(
            int orderId,
            string customerName = "Walk-in Customer",
            bool isFinalized = true)
            : base(orderId, customerName, isFinalized, OrderType.Takeaway)
        {
        }
    }
}
