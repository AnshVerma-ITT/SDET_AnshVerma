namespace GourmetSpot.Models
{
    public class CustomerOrder : Order
    {
        public override string OrderType => OrderTypes.Customer;

        public CustomerOrder(
            int orderId,
            string customerName = "Walk-in Customer",
            bool isFinalized = true)
            : base(orderId, customerName, isFinalized)
        {
        }
    }
}
