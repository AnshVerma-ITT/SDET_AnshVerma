using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IOrderManager
    {
        string LoadMessage { get; }

        int GetNextOrderId();
        List<Order> GetAllOrders();
        List<Order> GetActiveTableOrders();
        Order SearchActiveOrderByTable(int tableNumber);
        bool CreateOrder(
            Order order,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out string message);
        bool StartTableOrder(
            TableOrder order,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out string message);
        bool AddSubOrderToTable(
            int tableNumber,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out Order? order,
            out string message);
        bool FinalizeTableOrder(int tableNumber, out Order? order, out string message);
    }
}
