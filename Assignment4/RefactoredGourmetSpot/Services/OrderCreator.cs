using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Services
{
    public class OrderCreator : IOrderCreator
    {
        public Order CreateOrder(
            int orderId,
            string customerName,
            string orderType,
            int tableNumber,
            bool isFinalized)
        {
            if (orderType.Equals(OrderTypes.Table, StringComparison.OrdinalIgnoreCase) || tableNumber > 0)
            {
                return new TableOrder(orderId, customerName, tableNumber, isFinalized);
            }
            return new CustomerOrder(orderId, customerName, isFinalized);
        }
    }
}
