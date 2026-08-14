using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class OrderManagerTests : FileTestBase
    {
        [Test]
        public void CreateOrder_WhenTakeawayOrderIsValid_FinalizesAndSavesOrder()
        {
            int ingredientId = 1;
            double startingQuantity = 20;
            double requiredQuantity = 2;
            int orderQuantity = 2;
            InventoryManager inventoryManager = new InventoryManager();
            inventoryManager.AddIngredient(
                new Ingredient(ingredientId, "Ingredient", startingQuantity, "kg"),
                out _);
            OrderManager orderManager = new OrderManager();
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                100,
                new Dictionary<int, double> { { ingredientId, requiredQuantity } });
            OrderItem orderItem = new OrderItem(menuItem, orderQuantity);
            TakeawayOrder order = new TakeawayOrder(1, "Customer");
            bool created = orderManager.CreateOrder(
                order,
                new List<OrderItem> { orderItem },
                inventoryManager,
                out string message);
            Assert.That(created, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(order.IsFinalized, Is.True);
            Assert.That(order.SubOrders, Has.Count.EqualTo(1));
            Assert.That(order.Items, Has.Count.EqualTo(1));
            Assert.That(orderManager.GetAllOrders(), Has.Count.EqualTo(1));
            Assert.That(
                inventoryManager.SearchIngredientById(ingredientId)!.Quantity,
                Is.EqualTo(startingQuantity - requiredQuantity * orderQuantity));
        }

        [Test]
        public void CreateOrder_WhenItemsAreEmpty_ReturnsFalse()
        {
            InventoryManager inventoryManager = new InventoryManager();
            OrderManager orderManager = new OrderManager();
            TakeawayOrder order = new TakeawayOrder(1, "Customer");
            bool created = orderManager.CreateOrder(
                order,
                new List<OrderItem>(),
                inventoryManager,
                out string message);
            Assert.That(created, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void StartTableOrder_WhenTableAlreadyHasActiveOrder_ReturnsFalse()
        {
            int tableNumber = 4;
            InventoryManager inventoryManager = new InventoryManager();
            inventoryManager.AddIngredient(new Ingredient(1, "Ingredient", 20, "kg"), out _);
            OrderManager orderManager = new OrderManager();
            TableOrder firstOrder = new TableOrder(1, "Customer", tableNumber);
            TableOrder secondOrder = new TableOrder(2, "Another Customer", tableNumber);
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                100,
                new Dictionary<int, double> { { 1, 1 } });
            orderManager.StartTableOrder(
                firstOrder,
                new List<OrderItem> { new OrderItem(menuItem, 1) },
                inventoryManager,
                out _);
            bool started = orderManager.StartTableOrder(
                secondOrder,
                new List<OrderItem> { new OrderItem(menuItem, 1) },
                inventoryManager,
                out string message);
            Assert.That(started, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void AddSubOrderToTable_WhenTableOrderIsActive_AddsItems()
        {
            int tableNumber = 5;
            InventoryManager inventoryManager = new InventoryManager();
            inventoryManager.AddIngredient(new Ingredient(1, "Ingredient", 20, "kg"), out _);
            OrderManager orderManager = new OrderManager();
            TableOrder order = new TableOrder(1, "Customer", tableNumber);
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                100,
                new Dictionary<int, double> { { 1, 1 } });
            orderManager.StartTableOrder(
                order,
                new List<OrderItem> { new OrderItem(menuItem, 1) },
                inventoryManager,
                out _);
            int previousSubOrderCount = order.SubOrders.Count;
            int previousItemCount = order.Items.Count;
            List<OrderItem> newItems = new List<OrderItem> { new OrderItem(menuItem, 1) };
            bool added = orderManager.AddSubOrderToTable(
                tableNumber,
                newItems,
                inventoryManager,
                out Order? updatedOrder,
                out string message);
            Assert.That(added, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(updatedOrder, Is.Not.Null);
            Assert.That(updatedOrder!.SubOrders, Has.Count.EqualTo(previousSubOrderCount + 1));
            Assert.That(updatedOrder.Items, Has.Count.EqualTo(previousItemCount + newItems.Count));
        }

        [Test]
        public void FinalizeTableOrder_WhenTableOrderIsActive_FinalizesOrder()
        {
            int tableNumber = 5;
            InventoryManager inventoryManager = new InventoryManager();
            inventoryManager.AddIngredient(new Ingredient(1, "Ingredient", 20, "kg"), out _);
            OrderManager orderManager = new OrderManager();
            TableOrder order = new TableOrder(1, "Customer", tableNumber);
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                100,
                new Dictionary<int, double> { { 1, 1 } });
            orderManager.StartTableOrder(
                order,
                new List<OrderItem> { new OrderItem(menuItem, 1) },
                inventoryManager,
                out _);
            bool finalized = orderManager.FinalizeTableOrder(
                tableNumber,
                out Order? finalizedOrder,
                out string message);
            Assert.That(finalized, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(finalizedOrder, Is.Not.Null);
            Assert.That(finalizedOrder!.IsFinalized, Is.True);
            Assert.That(orderManager.SearchActiveOrderByTable(tableNumber), Is.Null);
        }

        [Test]
        public void FinalizeTableOrder_WhenNoActiveOrder_ReturnsFalse()
        {
            OrderManager orderManager = new OrderManager();
            bool finalized = orderManager.FinalizeTableOrder(
                1,
                out Order? finalizedOrder,
                out string message);
            Assert.That(finalized, Is.False);
            Assert.That(finalizedOrder, Is.Null);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void RebuildItemsFromSubOrders_WhenItemsAreOutOfDate_RebuildsItems()
        {
            TableOrder order = new TableOrder(1, "Customer", 2);
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                100,
                new Dictionary<int, double> { { 1, 1 } });
            OrderItem savedItem = new OrderItem(menuItem, 1);
            OrderItem oldItem = new OrderItem(menuItem, 1);
            SubOrder subOrder = new SubOrder(1, DateTime.UnixEpoch);
            subOrder.Items.Add(savedItem);
            order.SubOrders.Add(subOrder);
            order.Items.Add(oldItem);
            OrderManager.RebuildItemsFromSubOrders(order);
            Assert.That(order.Items, Has.Count.EqualTo(1));
            Assert.That(order.Items[0], Is.SameAs(savedItem));
        }
    }
}
