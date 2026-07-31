using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class BillManager : IBillManager
    {
        private const decimal GstRate = 0.18m;

        public Bill GenerateBill(Order order)
        {
            Bill bill = CreateBill(order);
            if (SaveBill(bill, out string? savedFilePath, out string saveMessage))
            {
                bill.IsSaved = true;
                bill.SavedFilePath = savedFilePath;
                bill.SaveMessage = saveMessage;
            }
            else
            {
                bill.IsSaved = false;
                bill.SavedFilePath = null;
                bill.SaveMessage = saveMessage;
            }
            return bill;
        }

        public Bill CreateBill(Order order)
        {
            decimal subtotal = CalculateSubtotal(order);
            decimal tax = CalculateTax(subtotal);
            decimal grandTotal = CalculateGrandTotal(subtotal, tax);
            Bill bill = new Bill(
                order.OrderId,
                order.CustomerName,
                order.TableNumber,
                CopySubOrders(order.SubOrders),
                new List<OrderItem>(order.Items),
                subtotal,
                tax,
                grandTotal);
            return bill;
        }

        private decimal CalculateSubtotal(Order order)
        {
            decimal subtotal = 0;
            foreach (OrderItem orderItem in order.Items)
            {
                subtotal += CalculateItemTotalPrice(orderItem);
            }
            return subtotal;
        }

        public decimal CalculateItemTotalPrice(OrderItem orderItem)
        {
            return orderItem.MenuItem.Price * orderItem.Quantity;
        }

        private decimal CalculateTax(decimal subtotal)
        {
            return subtotal * GstRate;
        }

        private decimal CalculateGrandTotal(decimal subtotal, decimal tax)
        {
            return subtotal + tax;
        }

        private bool SaveBill(Bill bill, out string? savedFilePath, out string saveMessage)
        {
            string billFilePath = FileManager.GetBillFilePath(bill.OrderId);
            List<string> billLines = new List<string>();
            billLines.Add("========== BILL ==========");
            billLines.Add($"Order ID : {bill.OrderId}");
            billLines.Add($"Customer Name : {bill.CustomerName}");
            if (bill.TableNumber > 0)
            {
                billLines.Add($"Table Number : {bill.TableNumber}");
            }
            billLines.Add("--------------------------");
            if (bill.SubOrders.Count > 0)
            {
                foreach (SubOrder subOrder in bill.SubOrders)
                {
                    billLines.Add(GetSubOrderHeading(subOrder));
                    foreach (OrderItem orderItem in subOrder.Items)
                    {
                        billLines.Add($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{CalculateItemTotalPrice(orderItem)}");
                    }
                    billLines.Add("--------------------------");
                }
            }
            else
            {
                foreach (OrderItem orderItem in bill.Items)
                {
                    billLines.Add($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{CalculateItemTotalPrice(orderItem)}");
                }
                billLines.Add("--------------------------");
            }
            billLines.Add($"Subtotal : ₹{bill.Subtotal}");
            billLines.Add($"GST (18%) : ₹{bill.Tax}");
            billLines.Add("--------------------------");
            billLines.Add($"Grand Total : ₹{bill.GrandTotal}");
            bool billSaved = FileManager.TryWriteAllLines(billFilePath, billLines);
            if (billSaved)
            {
                savedFilePath = billFilePath;
                saveMessage = $"Bill saved to file: {billFilePath}";
                return true;
            }
            savedFilePath = null;
            if (!string.IsNullOrWhiteSpace(FileManager.LastErrorMessage))
            {
                saveMessage = FileManager.LastErrorMessage;
            }
            else
            {
                saveMessage = "Bill could not be saved to file.";
            }
            return false;
        }

        private List<SubOrder> CopySubOrders(IReadOnlyList<SubOrder> subOrders)
        {
            List<SubOrder> copiedSubOrders = new List<SubOrder>();
            foreach (SubOrder subOrder in subOrders)
            {
                SubOrder copiedSubOrder = new SubOrder(subOrder.SubOrderNumber, subOrder.OrderedAt);
                foreach (OrderItem orderItem in subOrder.Items)
                {
                    copiedSubOrder.AddItem(orderItem);
                }
                copiedSubOrders.Add(copiedSubOrder);
            }
            return copiedSubOrders;
        }

        private string GetSubOrderHeading(SubOrder subOrder)
        {
            if (subOrder.OrderedAt == DateTime.MinValue)
            {
                return $"Suborder {subOrder.SubOrderNumber}";
            }
            return $"Suborder {subOrder.SubOrderNumber} ({subOrder.OrderedAt:dd-MM-yyyy HH:mm})";
        }
    }
}
