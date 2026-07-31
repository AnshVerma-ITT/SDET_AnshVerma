using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IOrderCreator
    {
        Order CreateOrder(
            int orderId,
            string customerName,
            string orderType,
            int tableNumber,
            bool isFinalized);
    }
}
