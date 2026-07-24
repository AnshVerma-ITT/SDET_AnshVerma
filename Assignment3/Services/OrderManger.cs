using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class OrderManager
    {
        private readonly List<Order> orders;
        private readonly string filePath = "Data/orders.txt";

        public OrderManager()
        {
            orders = new List<Order>();
            Directory.CreateDirectory("Data");
            LoadOrders();
        }

        public void CreateOrder(Order order)
        {
            orders.Add(order);
            SaveOrders();

            Console.WriteLine("\nOrder Placed Successfully.");
            Console.WriteLine($"Total Bill : ₹{order.CalculateGrandTotal()}");
        }

        public void DisplayOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No Orders Found.");
                return;
            }

            foreach (Order order in orders)
            {
                Console.WriteLine($"\nOrder ID : {order.OrderId}");

                foreach (OrderItem item in order.Items)
                {
                    Console.WriteLine($"{item.MenuItem.Name} x {item.Quantity} = ₹{item.TotalPrice}");
                }

                Console.WriteLine($"Total Bill : ₹{order.CalculateGrandTotal()}");
            }
        }

        private void SaveOrders()
        {
            List<string> lines = new List<string>();

            foreach (Order order in orders)
            {
                List<string> itemParts = new List<string>();

                foreach (OrderItem item in order.Items)
                {
                    itemParts.Add($"{item.MenuItem.MenuItemId}:{item.MenuItem.Name}:{item.MenuItem.Price}:{item.Quantity}");
                }

                lines.Add($"{order.OrderId}|{string.Join(";", itemParts)}");
            }

            File.WriteAllLines(filePath, lines);
        }

        private void LoadOrders()
        {
            if (!File.Exists(filePath))
                return;

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('|', 2);

                if (parts.Length < 2)
                    continue;

                int orderId = Convert.ToInt32(parts[0]);
                Order order = new Order(orderId);

                foreach (string itemData in parts[1].Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] itemParts = itemData.Split(':');

                    if (itemParts.Length < 4)
                        continue;

                    MenuItem menuItem = new MenuItem(
                        Convert.ToInt32(itemParts[0]),
                        itemParts[1],
                        Convert.ToDecimal(itemParts[2]),
                        new Dictionary<int, double>());

                    OrderItem orderItem = new OrderItem(menuItem, Convert.ToInt32(itemParts[3]));
                    order.AddItem(orderItem);
                }

                orders.Add(order);
            }
        }
    }
}