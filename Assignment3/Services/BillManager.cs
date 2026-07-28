using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class BillManager
    {
        private const decimal GstRate = 0.18m;

        public void GenerateBill(Order order)
        {
            decimal subtotal = order.CalculateSubtotal();
            decimal tax = CalculateTax(subtotal);
            decimal grandTotal = CalculateGrandTotal(subtotal, tax);
            string? billFilePath = SaveBill(order, subtotal, tax, grandTotal);

            Console.WriteLine();
            Console.WriteLine("========== BILL ==========");
            Console.WriteLine($"Order ID : {order.OrderId}");
            Console.WriteLine($"Customer Name : {order.CustomerName}");
            Console.WriteLine("--------------------------");

            foreach (OrderItem orderItem in order.Items)
            {
                Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{orderItem.TotalPrice}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine($"Subtotal : ₹{subtotal}");
            Console.WriteLine($"GST (18%): ₹{tax}");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Grand Total : ₹{grandTotal}");
            if (billFilePath != null)
            {
                Console.WriteLine($"Bill saved to file: {billFilePath}");
            }
            else
            {
                Console.WriteLine("Bill could not be saved to file.");
            }
        }

        private decimal CalculateTax(decimal subtotal)
        {
            return subtotal * GstRate;
        }

        private decimal CalculateGrandTotal(decimal subtotal, decimal tax)
        {
            return subtotal + tax;
        }

        private string? SaveBill(Order order, decimal subtotal, decimal tax, decimal grandTotal)
        {
            string billFilePath = ApplicationStorage.GetBillFilePath(order.OrderId);
            List<string> billLines = new List<string>();

            billLines.Add("========== BILL ==========");
            billLines.Add($"Order ID : {order.OrderId}");
            billLines.Add($"Customer Name : {order.CustomerName}");
            billLines.Add("--------------------------");

            foreach (OrderItem orderItem in order.Items)
            {
                billLines.Add($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{orderItem.TotalPrice}");
            }

            billLines.Add("--------------------------");
            billLines.Add($"Subtotal : ₹{subtotal}");
            billLines.Add($"GST (18%) : ₹{tax}");
            billLines.Add("--------------------------");
            billLines.Add($"Grand Total : ₹{grandTotal}");

            bool billSaved = ApplicationStorage.TryWriteAllLines(billFilePath, billLines);

            if (billSaved)
            {
                return billFilePath;
            }

            return null;
        }
    }
}
