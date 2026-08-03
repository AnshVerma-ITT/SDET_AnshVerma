using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class OrderManager : IOrderManager
    {
        public static void AddSubOrder(Order order, SubOrder subOrder)
        {
            if (order == null || subOrder == null) return;
            order.InternalSubOrders.Add(subOrder);
            foreach (OrderItem orderItem in subOrder.Items)
            {
                order.InternalItems.Add(orderItem);
            }
        }

        public static void RebuildItemsFromSubOrders(Order order)
        {
            if (order == null) return;
            order.InternalItems.Clear();
            foreach (SubOrder subOrder in order.InternalSubOrders)
            {
                foreach (OrderItem orderItem in subOrder.Items)
                {
                    order.InternalItems.Add(orderItem);
                }
            }
        }

        private readonly List<Order> orders;
        private readonly IOrderStore orderStore;
        private readonly IIngredientRequirementCalculator ingredientRequirementCalculator;

        public string LoadMessage => orderStore.LoadMessage;

        public OrderManager()
            : this(
                new OrderFileStore(new OrderCreator()),
                new IngredientRequirementCalculator())
        {
        }

        public OrderManager(
            IOrderStore orderStore,
            IIngredientRequirementCalculator ingredientRequirementCalculator)
        {
            this.orderStore = orderStore;
            this.ingredientRequirementCalculator = ingredientRequirementCalculator;
            orders = this.orderStore.LoadOrders();
        }

        public int GetNextOrderId()
        {
            int nextOrderId = 1;
            foreach (Order order in orders)
            {
                if (order.OrderId >= nextOrderId)
                {
                    nextOrderId = order.OrderId + 1;
                }
            }
            return nextOrderId;
        }

        public List<Order> GetAllOrders()
        {
            return new List<Order>(orders);
        }

        public List<Order> GetActiveTableOrders()
        {
            List<Order> activeTableOrders = new List<Order>();
            foreach (Order order in orders)
            {
                if (order.TableNumber > 0 && order.CanReceiveSubOrder)
                {
                    activeTableOrders.Add(order);
                }
            }
            return activeTableOrders;
        }

        public Order? SearchActiveOrderByTable(int tableNumber)
        {
            foreach (Order order in orders)
            {
                if (order.TableNumber == tableNumber && order.CanReceiveSubOrder)
                {
                    return order;
                }
            }
            return null;
        }

        public bool CreateOrder(
            Order order,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out string message)
        {
            if (order is TableOrder tableOrder)
            {
                return StartTableOrder(tableOrder, selectedMenuItems, inventoryManager, out message);
            }
            if (!ValidateOrderRequest(order, selectedMenuItems, inventoryManager, "Order was not placed.", out message))
            {
                return false;
            }
            if (!UseIngredientsForSelectedItems(
                selectedMenuItems,
                inventoryManager,
                "Order cannot be placed.",
                out message))
            {
                return false;
            }
            order.IsFinalized = true;
            AddSubOrderToOrder(order, selectedMenuItems);
            return PlaceOrder(
                order,
                "Order Placed Successfully.",
                "Order placed, but order data could not be saved.",
                out message);
        }

        public bool StartTableOrder(
            TableOrder order,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out string message)
        {
            if (!ValidateOrderRequest(order, selectedMenuItems, inventoryManager, "Table order was not started.", out message))
            {
                return false;
            }
            if (order.TableNumber <= 0)
            {
                message = "Table number must be greater than zero.";
                return false;
            }
            if (SearchActiveOrderByTable(order.TableNumber) != null)
            {
                message = "This table already has an active order. Please add a suborder or finalize the bill.";
                return false;
            }
            if (!UseIngredientsForSelectedItems(
                selectedMenuItems,
                inventoryManager,
                "Table order cannot be started.",
                out message))
            {
                return false;
            }
            order.IsFinalized = false;
            AddSubOrderToOrder(order, selectedMenuItems);
            return PlaceOrder(
                order,
                "Table order started successfully.",
                "Table order started, but order data could not be saved.",
                out message);
        }

        public bool AddSubOrderToTable(
            int tableNumber,
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            out Order? order,
            out string message)
        {
            order = SearchActiveOrderByTable(tableNumber);
            if (order == null)
            {
                message = "No active order found for this table.";
                return false;
            }
            if (!ValidateOrderRequest(order, selectedMenuItems, inventoryManager, "Suborder was not added.", out message))
            {
                return false;
            }
            if (!UseIngredientsForSelectedItems(
                selectedMenuItems,
                inventoryManager,
                "Suborder cannot be added.",
                out message))
            {
                return false;
            }
            AddSubOrderToOrder(order, selectedMenuItems);
            if (!SaveOrders())
            {
                message = GetStorageErrorMessage("Suborder added, but order data could not be saved.");
                return false;
            }
            message = "Suborder added successfully.";
            return true;
        }

        public bool FinalizeTableOrder(int tableNumber, out Order? order, out string message)
        {
            order = SearchActiveOrderByTable(tableNumber);
            if (order == null)
            {
                message = "No active order found for this table.";
                return false;
            }
            if (order.Items.Count == 0)
            {
                message = "Cannot finalize an order with no items.";
                return false;
            }
            order.IsFinalized = true;
            if (!SaveOrders())
            {
                order.IsFinalized = false;
                message = GetStorageErrorMessage("Table order finalized, but order data could not be saved.");
                return false;
            }
            message = "Table order finalized successfully.";
            return true;
        }

        private bool PlaceOrder(
            Order order,
            string successMessage,
            string saveErrorFallbackMessage,
            out string message)
        {
            orders.Add(order);
            if (!SaveOrders())
            {
                orders.Remove(order);
                message = GetStorageErrorMessage(saveErrorFallbackMessage);
                return false;
            }
            message = successMessage;
            return true;
        }

        private bool ValidateOrderRequest(
            Order? order,
            List<OrderItemSelection>? selectedMenuItems,
            IInventoryManager? inventoryManager,
            string emptySelectionMessage,
            out string message)
        {
            if (order == null)
            {
                message = "Order cannot be null.";
                return false;
            }
            if (selectedMenuItems == null || selectedMenuItems.Count == 0)
            {
                message = emptySelectionMessage;
                return false;
            }
            if (inventoryManager == null)
            {
                message = "Inventory manager is not available.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool UseIngredientsForSelectedItems(
            List<OrderItemSelection> selectedMenuItems,
            IInventoryManager inventoryManager,
            string actionBlockedMessage,
            out string message)
        {
            Dictionary<int, double> requiredIngredients =
                ingredientRequirementCalculator.CalculateRequiredIngredients(selectedMenuItems);
            if (!inventoryManager.UseIngredients(requiredIngredients, out string inventoryMessage))
            {
                if (string.IsNullOrWhiteSpace(inventoryMessage))
                {
                    message = actionBlockedMessage;
                }
                else
                {
                    message = $"{inventoryMessage}\n{actionBlockedMessage}";
                }
                return false;
            }
            message = string.Empty;
            return true;
        }

        private void AddSubOrderToOrder(Order order, List<OrderItemSelection> selectedMenuItems)
        {
            SubOrder subOrder = new SubOrder(GetNextSubOrderNumber(order), DateTime.Now);
            foreach (OrderItemSelection selectedMenuItem in selectedMenuItems)
            {
                subOrder.AddItem(selectedMenuItem.ToOrderItem());
            }
                AddSubOrder(order, subOrder);
        }

        private int GetNextSubOrderNumber(Order order)
        {
            int nextSubOrderNumber = 1;
            foreach (SubOrder subOrder in order.SubOrders)
            {
                if (subOrder.SubOrderNumber >= nextSubOrderNumber)
                {
                    nextSubOrderNumber = subOrder.SubOrderNumber + 1;
                }
            }
            return nextSubOrderNumber;
        }

        private bool SaveOrders()
        {
            return orderStore.SaveOrders(orders);
        }

        private static string GetStorageErrorMessage(string fallbackMessage)
        {
            if (!string.IsNullOrWhiteSpace(FileManager.LastErrorMessage))
            {
                return FileManager.LastErrorMessage;
            }
            return fallbackMessage;
        }
    }
}
