using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class OrderStoreManagerTests : FileTestBase
    {
        [Test]
        public void SaveAndLoad_WhenOrderHasSubOrders_RestoresOrder()
        {
            OrderStoreManager storeManager = new OrderStoreManager();
            TableOrder order = new TableOrder(1, "Customer", 1);
            OrderItem orderItem = new OrderItem(
                new MenuItem(1, "Menu Item", 80, new Dictionary<int, double>()),
                2);
            SubOrder subOrder = new SubOrder(1, DateTime.UnixEpoch);
            subOrder.Items.Add(orderItem);
            OrderManager.AddSubOrder(order, subOrder);
            bool saved = storeManager.Save(new List<Order> { order });
            List<Order> loadedOrders = storeManager.Load();
            Assert.That(saved, Is.True);
            Assert.That(loadedOrders, Has.Count.EqualTo(1));
            Assert.That(loadedOrders[0].OrderId, Is.EqualTo(order.OrderId));
            Assert.That(loadedOrders[0].CustomerName, Is.EqualTo(order.CustomerName));
            Assert.That(loadedOrders[0].OrderType, Is.EqualTo(order.OrderType));
            Assert.That(loadedOrders[0].TableNumber, Is.EqualTo(order.TableNumber));
            Assert.That(loadedOrders[0].IsFinalized, Is.EqualTo(order.IsFinalized));
            Assert.That(loadedOrders[0].SubOrders, Has.Count.EqualTo(order.SubOrders.Count));
            Assert.That(loadedOrders[0].Items, Has.Count.EqualTo(order.Items.Count));
            Assert.That(loadedOrders[0].Items[0].MenuItem.Name, Is.EqualTo(orderItem.MenuItem.Name));
        }
    }
}
