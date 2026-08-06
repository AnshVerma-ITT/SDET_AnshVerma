using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IBillManager
    {
        Bill GenerateBill(Order order);
        decimal CalculateItemTotalPrice(OrderItem orderItem);
    }
}
