using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class BillManager
    {
        private const decimal GST = 0.18m;

        public void GenerateBill(Order order)
        {
            decimal subTotal = order.CalculateGrandTotal();

            decimal tax = subTotal * GST;

            decimal grandTotal = subTotal + tax;

            Console.WriteLine();
            Console.WriteLine("========== BILL ==========");

            foreach (OrderItem item in order.Items)
            {
                Console.WriteLine($"{item.MenuItem.Name} x {item.Quantity} = ₹{item.TotalPrice}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine($"Subtotal : ₹{subTotal}");
            Console.WriteLine($"GST (18%): ₹{tax}");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Grand Total : ₹{grandTotal}");

            SaveBill(order, subTotal, tax, grandTotal);
        }

        private void SaveBill(Order order, decimal subTotal, decimal tax, decimal grandTotal)
        {

            string filePath = $"Data/Bill_{order.OrderId}.txt";

            List<string> lines = new List<string>();

            lines.Add("========== BILL ==========");

            foreach (OrderItem item in order.Items)
            {
                lines.Add($"{item.MenuItem.Name} x {item.Quantity} = ₹{item.TotalPrice}");
            }

            lines.Add("--------------------------");
            lines.Add($"Subtotal : ₹{subTotal}");
            lines.Add($"GST (18%) : ₹{tax}");
            lines.Add($"Grand Total : ₹{grandTotal}");

            File.WriteAllLines(filePath, lines);
        }
    }
}