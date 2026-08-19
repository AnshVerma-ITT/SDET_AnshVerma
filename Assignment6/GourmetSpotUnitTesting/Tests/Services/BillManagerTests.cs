using GourmetSpot.Exceptions;
using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class BillManagerTests
    {
        [Test]
        public void CreateBill_WhenOrderHasItems_CalculatesTotals()
        {
            BillManager manager = new BillManager();
            TableOrder order = TestData.CreateTableOrder(isFinalized: true);
            order.Items.Add(TestData.CreateOrderItem(
                TestData.CreateMenuItem(name: TestData.MainItemName)));
            order.Items.Add(TestData.CreateOrderItem(
                TestData.CreateMenuItem(
                    menuItemId: TestData.SecondId,
                    name: TestData.SideItemName,
                    price: TestData.SideItemPrice),
                TestData.SingleQuantity));
            decimal expectedSubtotal = decimal.Zero;
            foreach (OrderItem orderItem in order.Items)
            {
                expectedSubtotal += orderItem.MenuItem.Price * orderItem.Quantity;
            }
            decimal expectedTax = expectedSubtotal * TestData.GstRate;
            decimal expectedGrandTotal = expectedSubtotal + expectedTax;
            Bill bill = manager.CreateBill(order);
            Assert.That(bill.Subtotal, Is.EqualTo(expectedSubtotal), "CreateBill should calculate subtotal from all order items.");
            Assert.That(bill.Tax, Is.EqualTo(expectedTax), "CreateBill should calculate GST using the configured tax rate.");
            Assert.That(bill.GrandTotal, Is.EqualTo(expectedGrandTotal), "CreateBill should calculate grand total as subtotal plus tax.");
        }

        [Test]
        public void CreateBill_WhenOrderHasDetails_CopiesOrderDetails()
        {
            BillManager manager = new BillManager();
            TableOrder order = TestData.CreateTableOrder(isFinalized: true);
            OrderItem orderItem = TestData.CreateOrderItem(quantity: TestData.SingleQuantity);
            order.Items.Add(orderItem);
            Bill bill = manager.CreateBill(order);
            Assert.That(bill.OrderId, Is.EqualTo(order.OrderId), "CreateBill should copy the order id into the bill.");
            Assert.That(bill.CustomerName, Is.EqualTo(order.CustomerName), "CreateBill should copy the customer name into the bill.");
            Assert.That(bill.TableNumber, Is.EqualTo(order.TableNumber), "CreateBill should copy the table number into the bill.");
            Assert.That(bill.Items, Has.Count.EqualTo(order.Items.Count), "CreateBill should copy all order items into the bill.");
            Assert.That(bill.Items[TestData.FirstIndex], Is.SameAs(orderItem), "CreateBill should keep the same order item reference in bill items.");
        }

        [Test]
        public void CreateBill_WhenOrderHasSubOrders_CopiesSubOrders()
        {
            BillManager manager = new BillManager();
            TableOrder order = TestData.CreateTableOrder(isFinalized: true);
            SubOrder subOrder = TestData.CreateSubOrder();
            OrderItem orderItem = TestData.CreateOrderItem(quantity: TestData.SingleQuantity);
            subOrder.Items.Add(orderItem);
            order.SubOrders.Add(subOrder);
            Bill bill = manager.CreateBill(order);
            Assert.That(bill.SubOrders, Has.Count.EqualTo(order.SubOrders.Count), "CreateBill should copy every suborder into the bill.");
            Assert.That(bill.SubOrders[TestData.FirstIndex], Is.Not.SameAs(subOrder), "CreateBill should create a separate suborder copy for the bill.");
            Assert.That(bill.SubOrders[TestData.FirstIndex].Items[TestData.FirstIndex], Is.SameAs(orderItem), "CreateBill should keep suborder item references in the copied suborder.");
        }

        [Test]
        public void CalculateItemTotalPrice_WhenItemHasQuantity_ReturnsPriceTimesQuantity()
        {
            BillManager manager = new BillManager();
            OrderItem orderItem = TestData.CreateOrderItem();
            decimal expectedTotalPrice = orderItem.MenuItem.Price * orderItem.Quantity;
            decimal totalPrice = manager.CalculateItemTotalPrice(orderItem);
            Assert.That(totalPrice, Is.EqualTo(expectedTotalPrice), "CalculateItemTotalPrice should multiply menu item price by quantity.");
        }

        [Test]
        public void GenerateBill_WhenOrderIsNull_ThrowsBillException()
        {
            BillManager manager = new BillManager();
            Assert.Throws<BillException>(() => manager.GenerateBill(null!), "GenerateBill should throw BillException when order is null.");
        }
    }
}
