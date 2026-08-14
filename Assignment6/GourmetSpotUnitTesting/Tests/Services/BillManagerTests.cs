using GourmetSpot.Exceptions;
using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;
using GourmetSpot.Utilities;

namespace GourmetSpot.Tests.Services
{
    public class BillManagerTests : FileTestBase
    {
        [Test]
        public void CreateBill_WhenOrderHasItems_CalculatesTotals()
        {
            BillManager manager = new BillManager();
            TableOrder order = new TableOrder(1, "Customer", 1, true);
            SubOrder subOrder = new SubOrder(1, DateTime.UnixEpoch);
            subOrder.Items.Add(new OrderItem(
                new MenuItem(1, "Main Item", 100, new Dictionary<int, double>()),
                2));
            subOrder.Items.Add(new OrderItem(
                new MenuItem(2, "Side Item", 50, new Dictionary<int, double>()),
                1));
            OrderManager.AddSubOrder(order, subOrder);
            decimal expectedSubtotal = 0;
            foreach (OrderItem orderItem in order.Items)
            {
                expectedSubtotal += orderItem.MenuItem.Price * orderItem.Quantity;
            }
            decimal expectedTax = expectedSubtotal * 0.18m;
            decimal expectedGrandTotal = expectedSubtotal + expectedTax;
            Bill bill = manager.CreateBill(order);
            Assert.That(bill.OrderId, Is.EqualTo(order.OrderId));
            Assert.That(bill.CustomerName, Is.EqualTo(order.CustomerName));
            Assert.That(bill.TableNumber, Is.EqualTo(order.TableNumber));
            Assert.That(bill.Subtotal, Is.EqualTo(expectedSubtotal));
            Assert.That(bill.Tax, Is.EqualTo(expectedTax));
            Assert.That(bill.GrandTotal, Is.EqualTo(expectedGrandTotal));
            Assert.That(bill.Items, Has.Count.EqualTo(order.Items.Count));
            Assert.That(bill.SubOrders, Has.Count.EqualTo(order.SubOrders.Count));
        }

        [Test]
        public void GenerateBill_WhenOrderIsValid_SavesBillFile()
        {
            BillManager manager = new BillManager();
            TableOrder order = new TableOrder(1, "Customer", 1, true);
            SubOrder subOrder = new SubOrder(1, DateTime.UnixEpoch);
            subOrder.Items.Add(new OrderItem(
                new MenuItem(1, "Main Item", 100, new Dictionary<int, double>()),
                2));
            subOrder.Items.Add(new OrderItem(
                new MenuItem(2, "Side Item", 50, new Dictionary<int, double>()),
                1));
            OrderManager.AddSubOrder(order, subOrder);
            string expectedFilePath = FileManager.GetBillFilePath(order.OrderId);
            Bill bill = manager.GenerateBill(order);
            Assert.That(bill.IsSaved, Is.True);
            Assert.That(bill.SavedFilePath, Is.EqualTo(expectedFilePath));
            Assert.That(File.Exists(expectedFilePath), Is.True);
        }

        [Test]
        public void GenerateBill_WhenOrderIsNull_ThrowsBillException()
        {
            BillManager manager = new BillManager();
            Assert.Throws<BillException>(() => manager.GenerateBill(null!));
        }
    }
}
