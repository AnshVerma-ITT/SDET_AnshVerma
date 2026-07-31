using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IBillManager
    {
        Bill GenerateBill(Order order);
        Bill CreateBill(Order order);
        decimal CalculateItemTotalPrice(OrderItem orderItem);
    }
}
