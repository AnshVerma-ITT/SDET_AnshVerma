using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IOrderStore
    {
        string LoadMessage { get; }

        List<Order> LoadOrders();
        bool SaveOrders(List<Order> orders);
    }
}
