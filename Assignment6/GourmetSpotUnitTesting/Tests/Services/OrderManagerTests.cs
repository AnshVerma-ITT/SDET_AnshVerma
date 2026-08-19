using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Fakes;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class OrderManagerTests
    {
        [Test]
        public void GetNextOrderId_WhenOrdersExist_ReturnsCountPlusOne()
        {
            List<Order> savedOrders = new List<Order>
            {
                TestData.CreateTakeawayOrder()
            };
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(
                savedOrders);
            OrderManager orderManager = new OrderManager(orderStore);
            int nextId = orderManager.GetNextOrderId();
            Assert.That(nextId, Is.EqualTo(savedOrders.Count + TestData.FirstId), "GetNextOrderId should return current order count plus one.");
        }

        [Test]
        public void GetAllOrders_WhenOrdersExist_ReturnsCopy()
        {
            Order order = TestData.CreateTakeawayOrder();
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(new List<Order> { order });
            OrderManager orderManager = new OrderManager(orderStore);
            List<Order> orders = orderManager.GetAllOrders();
            Assert.That(orders, Is.Not.SameAs(orderManager.orders), "GetAllOrders should return a new list instead of the internal order list.");
            Assert.That(orders, Has.Count.EqualTo(orderManager.orders.Count), "GetAllOrders should include all saved orders.");
            Assert.That(orders[TestData.FirstIndex], Is.SameAs(order), "GetAllOrders should return the saved order item.");
        }

        [Test]
        public void GetActiveTableOrders_WhenOnlyOneTableOrderIsActive_ReturnsActiveOrder()
        {
            TableOrder activeOrder = TestData.CreateTableOrder();
            TableOrder finalizedOrder = TestData.CreateTableOrder(
                orderId: TestData.SecondId,
                tableNumber: TestData.OtherTableNumber,
                isFinalized: true);
            TakeawayOrder takeawayOrder = TestData.CreateTakeawayOrder(orderId: TestData.ThirdId);
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(
                new List<Order> { activeOrder, finalizedOrder, takeawayOrder });
            OrderManager orderManager = new OrderManager(orderStore);
            List<Order> activeOrders = orderManager.GetActiveTableOrders();
            Assert.That(activeOrders, Has.Count.EqualTo(TestData.SingleQuantity), "GetActiveTableOrders should return only active table orders.");
            Assert.That(activeOrders[TestData.FirstIndex], Is.SameAs(activeOrder), "GetActiveTableOrders should include the active table order.");
        }

        [Test]
        public void SearchActiveOrderByTable_WhenActiveOrderExists_ReturnsOrder()
        {
            TableOrder order = TestData.CreateTableOrder();
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(new List<Order> { order });
            OrderManager orderManager = new OrderManager(orderStore);
            Order? foundOrder = orderManager.SearchActiveOrderByTable(order.TableNumber);
            Assert.That(foundOrder, Is.SameAs(order), "SearchActiveOrderByTable should return the active order for the requested table.");
        }

        [Test]
        public void AddSubOrder_WhenOrderAndSubOrderAreValid_AddsSubOrderAndItems()
        {
            TableOrder order = TestData.CreateTableOrder();
            SubOrder subOrder = TestData.CreateSubOrder();
            OrderItem orderItem = TestData.CreateOrderItem();
            subOrder.Items.Add(orderItem);
            OrderManager.AddSubOrder(order, subOrder);
            Assert.That(order.SubOrders, Has.Count.EqualTo(TestData.SingleQuantity), "AddSubOrder should add the suborder to the order.");
            Assert.That(order.SubOrders[TestData.FirstIndex], Is.SameAs(subOrder), "AddSubOrder should keep the same suborder reference.");
            Assert.That(order.Items, Has.Count.EqualTo(subOrder.Items.Count), "AddSubOrder should copy suborder items into order items.");
            Assert.That(order.Items[TestData.FirstIndex], Is.SameAs(orderItem), "AddSubOrder should keep the same order item reference.");
        }

        [Test]
        public void CreateOrder_WhenTakeawayOrderIsValid_FinalizesAndSavesOrder()
        {
            int ingredientId = TestData.FirstId;
            double startingQuantity = TestData.StockQuantity;
            double requiredQuantity = TestData.RequiredQuantity;
            int orderQuantity = TestData.OrderQuantity;
            FakeInventoryManager inventoryManager = new FakeInventoryManager(
                new List<Ingredient>
                {
                    TestData.CreateIngredient(ingredientId: ingredientId, quantity: startingQuantity)
                });
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager();
            OrderManager orderManager = new OrderManager(orderStore);
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe: TestData.CreateRecipe(ingredientId, requiredQuantity));
            OrderItem orderItem = TestData.CreateOrderItem(menuItem, orderQuantity);
            TakeawayOrder order = TestData.CreateTakeawayOrder();
            bool created = orderManager.CreateOrder(
                order,
                new List<OrderItem> { orderItem },
                inventoryManager,
                out string message);
            Assert.That(created, Is.True, "CreateOrder should return true for a valid takeaway order.");
            Assert.That(message, Is.Not.Empty, "CreateOrder should return a success message for a valid takeaway order.");
            Assert.That(order.IsFinalized, Is.True, "CreateOrder should finalize takeaway orders.");
            Assert.That(order.SubOrders, Has.Count.EqualTo(TestData.SingleQuantity), "CreateOrder should create one suborder for the selected items.");
            Assert.That(order.Items, Has.Count.EqualTo(TestData.SingleQuantity), "CreateOrder should add selected items to the order.");
            Assert.That(orderStore.SaveWasCalled, Is.True, "CreateOrder should save the order through the injected order store.");
            Assert.That(
                inventoryManager.SearchIngredientById(ingredientId)!.Quantity,
                Is.EqualTo(startingQuantity - requiredQuantity * orderQuantity),
                "CreateOrder should reduce inventory by required recipe quantity times order quantity.");
        }

        [Test]
        public void CreateOrder_WhenItemsAreEmpty_ReturnsFalse()
        {
            FakeInventoryManager inventoryManager = new FakeInventoryManager(new List<Ingredient>());
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager();
            OrderManager orderManager = new OrderManager(orderStore);
            TakeawayOrder order = TestData.CreateTakeawayOrder();
            bool created = orderManager.CreateOrder(
                order,
                new List<OrderItem>(),
                inventoryManager,
                out string message);
            Assert.That(created, Is.False, "CreateOrder should return false when selected items are empty.");
            Assert.That(message, Is.Not.Empty, "CreateOrder should return a validation message when selected items are empty.");
            Assert.That(orderStore.SaveWasCalled, Is.False, "CreateOrder should not save when selected items are empty.");
        }

        [Test]
        public void StartTableOrder_WhenTableAlreadyHasActiveOrder_ReturnsFalse()
        {
            int tableNumber = TestData.TableNumber;
            FakeInventoryManager inventoryManager = new FakeInventoryManager(
                new List<Ingredient> { TestData.CreateIngredient() });
            TableOrder firstOrder = TestData.CreateTableOrder(tableNumber: tableNumber);
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(new List<Order> { firstOrder });
            OrderManager orderManager = new OrderManager(orderStore);
            TableOrder secondOrder = TestData.CreateTableOrder(
                orderId: TestData.SecondId,
                customerName: TestData.OtherCustomerName,
                tableNumber: tableNumber);
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe: TestData.CreateRecipe(requiredQuantity: TestData.SingleQuantity));
            bool started = orderManager.StartTableOrder(
                secondOrder,
                new List<OrderItem> { TestData.CreateOrderItem(menuItem, TestData.SingleQuantity) },
                inventoryManager,
                out string message);
            Assert.That(started, Is.False, "StartTableOrder should reject a table that already has an active order.");
            Assert.That(message, Is.Not.Empty, "StartTableOrder should return a validation message when the table is already active.");
        }

        [Test]
        public void AddSubOrderToTable_WhenTableOrderIsActive_AddsItems()
        {
            int tableNumber = TestData.TableNumber;
            FakeInventoryManager inventoryManager = new FakeInventoryManager(
                new List<Ingredient> { TestData.CreateIngredient() });
            TableOrder order = TestData.CreateTableOrder(tableNumber: tableNumber);
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(new List<Order> { order });
            OrderManager orderManager = new OrderManager(orderStore);
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe: TestData.CreateRecipe(requiredQuantity: TestData.SingleQuantity));
            int previousSubOrderCount = order.SubOrders.Count;
            int previousItemCount = order.Items.Count;
            List<OrderItem> newItems = new List<OrderItem>
            {
                TestData.CreateOrderItem(menuItem, TestData.SingleQuantity)
            };
            bool added = orderManager.AddSubOrderToTable(
                tableNumber,
                newItems,
                inventoryManager,
                out Order? updatedOrder,
                out string message);
            Assert.That(added, Is.True, "AddSubOrderToTable should return true for an active table order.");
            Assert.That(message, Is.Not.Empty, "AddSubOrderToTable should return a success message when a suborder is added.");
            Assert.That(updatedOrder, Is.Not.Null, "AddSubOrderToTable should return the updated active order.");
            Assert.That(
                updatedOrder!.SubOrders,
                Has.Count.EqualTo(previousSubOrderCount + TestData.SingleQuantity),
                "AddSubOrderToTable should increase the suborder count by one.");
            Assert.That(updatedOrder.Items, Has.Count.EqualTo(previousItemCount + newItems.Count), "AddSubOrderToTable should add new items to the active order.");
        }

        [Test]
        public void FinalizeTableOrder_WhenTableOrderIsActive_FinalizesOrder()
        {
            int tableNumber = TestData.TableNumber;
            TableOrder order = TestData.CreateTableOrder(tableNumber: tableNumber);
            SubOrder subOrder = TestData.CreateSubOrder();
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe: TestData.CreateRecipe(requiredQuantity: TestData.SingleQuantity));
            OrderItem orderItem = TestData.CreateOrderItem(menuItem, TestData.SingleQuantity);
            subOrder.Items.Add(orderItem);
            order.SubOrders.Add(subOrder);
            order.Items.Add(orderItem);
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager(new List<Order> { order });
            OrderManager orderManager = new OrderManager(orderStore);
            bool finalized = orderManager.FinalizeTableOrder(
                tableNumber,
                out Order? finalizedOrder,
                out string message);
            Assert.That(finalized, Is.True, "FinalizeTableOrder should return true for an active table order with items.");
            Assert.That(message, Is.Not.Empty, "FinalizeTableOrder should return a success message when finalized.");
            Assert.That(finalizedOrder, Is.Not.Null, "FinalizeTableOrder should return the finalized order.");
            Assert.That(finalizedOrder!.IsFinalized, Is.True, "FinalizeTableOrder should mark the order as finalized.");
            Assert.That(orderStore.SaveWasCalled, Is.True, "FinalizeTableOrder should save the finalized order.");
        }

        [Test]
        public void FinalizeTableOrder_WhenNoActiveOrder_ReturnsFalse()
        {
            FakeOrderStoreManager orderStore = new FakeOrderStoreManager();
            OrderManager orderManager = new OrderManager(orderStore);
            bool finalized = orderManager.FinalizeTableOrder(
                TestData.TableNumber,
                out Order? finalizedOrder,
                out string message);
            Assert.That(finalized, Is.False, "FinalizeTableOrder should return false when no active order exists.");
            Assert.That(finalizedOrder, Is.Null, "FinalizeTableOrder should not return an order when no active order exists.");
            Assert.That(message, Is.Not.Empty, "FinalizeTableOrder should return a message when no active order exists.");
        }

        [Test]
        public void RebuildItemsFromSubOrders_WhenItemsAreOutOfDate_RebuildsItems()
        {
            TableOrder order = TestData.CreateTableOrder();
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe: TestData.CreateRecipe(requiredQuantity: TestData.SingleQuantity));
            OrderItem savedItem = TestData.CreateOrderItem(menuItem, TestData.SingleQuantity);
            OrderItem oldItem = TestData.CreateOrderItem(menuItem, TestData.SingleQuantity);
            SubOrder subOrder = TestData.CreateSubOrder();
            subOrder.Items.Add(savedItem);
            order.SubOrders.Add(subOrder);
            order.Items.Add(oldItem);
            OrderManager.RebuildItemsFromSubOrders(order);
            Assert.That(order.Items, Has.Count.EqualTo(subOrder.Items.Count), "RebuildItemsFromSubOrders should replace stale items with suborder items.");
            Assert.That(order.Items[TestData.FirstIndex], Is.SameAs(savedItem), "RebuildItemsFromSubOrders should keep the item from the suborder.");
        }
    }
}
