using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Services.Contracts;
using GourmetSpot.UserInterface.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class OrderScreen : IDisplay
    {
        private OrderManager orderManager;
        private MenuManager menuManager;
        private IInventoryManager inventoryManager;
        private IBillManager billManager;

        public OrderScreen(
            OrderManager orderManager,
            MenuManager menuManager,
            IInventoryManager inventoryManager,
            IBillManager billManager)
        {
            this.orderManager = orderManager;
            this.menuManager = menuManager;
            this.inventoryManager = inventoryManager;
            this.billManager = billManager;
        }

        public void Display()
        {
            while (true)
            {
                DisplayList();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        TakeOrder();
                        break;
                    case "2":
                        DisplayOrders();
                        break;
                    case "3":
                        return;
                    case "4":
                        StartTableOrder();
                        break;
                    case "5":
                        AddSubOrderToTable();
                        break;
                    case "6":
                        FinalizeTableOrderBill();
                        break;
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }
        }

        public void DisplayList()
        {
            Console.WriteLine();
            Console.WriteLine("===== Order Management =====");
            Console.WriteLine("1. Create Order");
            Console.WriteLine("2. View Orders");
            Console.WriteLine("3. Back");
            Console.WriteLine("4. Start Table Order");
            Console.WriteLine("5. Add Suborder To Table");
            Console.WriteLine("6. Finalize Table Bill");
            Console.Write("Enter Choice : ");
        }

        private void TakeOrder()
        {
            if (!HasMenuItems())
            {
                return;
            }
            int orderId = orderManager.GetNextOrderId();
            Console.WriteLine($"Order ID: {orderId}");
            string customerName = ConsoleInput.ReadRequiredText("Enter Customer Name: ");
            TakeawayOrder order = new TakeawayOrder(orderId, customerName);
            List<OrderItem> selectedMenuItems = ReadSelectedMenuItems("Order Items");
            if (!orderManager.CreateOrder(order, selectedMenuItems, inventoryManager, out string orderMessage))
            {
                Console.WriteLine(orderMessage);
                return;
            }
            Console.WriteLine();
            Console.WriteLine(orderMessage);
            Bill bill = billManager.GenerateBill(order);
            if (!bill.IsSaved)
            {
                Console.WriteLine(bill.SaveMessage);
                return;
            }
            Console.WriteLine($"Order Subtotal : ₹{bill.Subtotal}");
            DisplayBill(bill);
        }

        private void StartTableOrder()
        {
            if (!HasMenuItems())
            {
                return;
            }
            int orderId = orderManager.GetNextOrderId();
            Console.WriteLine($"Order ID: {orderId}");
            string customerName = ConsoleInput.ReadRequiredText("Enter Customer Name: ");
            int tableNumber = ConsoleInput.ReadPositiveInt("Enter Table Number: ");
            TableOrder order = new TableOrder(orderId, customerName, tableNumber, false);
            List<OrderItem> selectedMenuItems = ReadSelectedMenuItems("Suborder 1");
            if (!orderManager.StartTableOrder(order, selectedMenuItems, inventoryManager, out string orderMessage))
            {
                Console.WriteLine(orderMessage);
                return;
            }
            Console.WriteLine();
            Console.WriteLine(orderMessage);
            DisplayOrder(order);
        }

        private void AddSubOrderToTable()
        {
            if (!HasMenuItems())
            {
                return;
            }
            if (!DisplayActiveTableOrders())
            {
                return;
            }
            int tableNumber = ConsoleInput.ReadPositiveInt("Enter Table Number: ");
            List<OrderItem> selectedMenuItems = ReadSelectedMenuItems("New Suborder");
            if (!orderManager.AddSubOrderToTable(
                tableNumber,
                selectedMenuItems,
                inventoryManager,
                out Order? order,
                out string orderMessage))
            {
                Console.WriteLine(orderMessage);
                return;
            }
            Console.WriteLine();
            Console.WriteLine(orderMessage);
            if (order != null)
            {
                DisplayOrder(order);
            }
        }

        private void FinalizeTableOrderBill()
        {
            if (!DisplayActiveTableOrders())
            {
                return;
            }
            int tableNumber = ConsoleInput.ReadPositiveInt("Enter Table Number: ");
            Order? order = orderManager.SearchActiveOrderByTable(tableNumber);
            if (order == null)
            {
                Console.WriteLine("No active order found for this table.");
                return;
            }
            Bill bill = billManager.GenerateBill(order);
            if (!bill.IsSaved)
            {
                Console.WriteLine(bill.SaveMessage);
                Console.WriteLine("Table order is still open. Fix the bill save issue and finalize again.");
                return;
            }
            if (!orderManager.FinalizeTableOrder(tableNumber, out _, out string orderMessage))
            {
                Console.WriteLine(orderMessage);
                Console.WriteLine(bill.SaveMessage);
                return;
            }
            Console.WriteLine();
            Console.WriteLine(orderMessage);
            DisplayBill(bill);
        }

        private bool HasMenuItems()
        {
            if (menuManager.GetAllMenuItems().Count == 0)
            {
                Console.WriteLine("Please add menu items before creating an order.");
                return false;
            }
            return true;
        }

        private bool DisplayActiveTableOrders()
        {
            List<Order> activeTableOrders = orderManager.GetActiveTableOrders();
            if (activeTableOrders.Count == 0)
            {
                Console.WriteLine("No active table orders found.");
                return false;
            }
            Console.WriteLine("\nActive Table Orders:");
            foreach (Order order in activeTableOrders)
            {
                Bill bill = billManager.GenerateBill(order);
                Console.WriteLine(
                    $"Table {order.TableNumber} - Order {order.OrderId} - {order.CustomerName} - Suborders: {order.SubOrders.Count} - Current Subtotal: ₹{bill.Subtotal}");
            }
            return true;
        }

        private List<OrderItem> ReadSelectedMenuItems(string heading)
        {
            List<OrderItem> selectedMenuItems = new();
            Console.WriteLine($"\n--- {heading} ---");
            DisplayMenu();
            while (true)
            {
                int menuItemId = ConsoleInput.ReadInt("Enter Menu Item ID (0 to finish): ");
                if (menuItemId == 0)
                {
                    break;
                }
                MenuItem? menuItem = menuManager.SearchMenuItemById(menuItemId);
                if (menuItem == null)
                {
                    Console.WriteLine("Menu Item Not Found.");
                    continue;
                }
                int quantity = ConsoleInput.ReadPositiveInt("Enter Quantity: ");
                selectedMenuItems.Add(new OrderItem(menuItem, quantity));
                Console.WriteLine($"{menuItem.Name} added to order.");
            }
            return selectedMenuItems;
        }

        private void DisplayOrders()
        {
            List<Order> orders = orderManager.GetAllOrders();
            if (orders.Count == 0)
            {
                Console.WriteLine("No Orders Found.");
                return;
            }
            foreach (Order order in orders)
            {
                DisplayOrder(order);
            }
        }

        private void DisplayOrder(Order order)
        {
            Bill bill = billManager.GenerateBill(order);
            Console.WriteLine($"\nOrder ID : {order.OrderId}");
            Console.WriteLine($"Customer Name : {order.CustomerName}");
            if (order.TableNumber > 0)
            {
                Console.WriteLine($"Table Number : {order.TableNumber}");
            }
            Console.WriteLine($"Status : {(order.IsFinalized ? "Finalized" : "Open")}");
            if (order.SubOrders.Count > 0)
            {
                foreach (SubOrder subOrder in order.SubOrders)
                {
                    DisplaySubOrder(subOrder);
                }
            }
            else
            {
                foreach (OrderItem orderItem in bill.Items)
                {
                    Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{billManager.CalculateItemTotalPrice(orderItem)}");
                }
            }
            Console.WriteLine($"Order Subtotal : ₹{bill.Subtotal}");
        }

        private void DisplayMenu()
        {
            List<MenuItem> menuItems = menuManager.GetAllMenuItems();
            if (menuItems.Count == 0)
            {
                Console.WriteLine("Menu is Empty.");
                return;
            }
            Console.WriteLine("\n========== MENU ==========");
            foreach (MenuItem menuItem in menuItems)
            {
                Console.WriteLine($"{menuItem.MenuItemId} - {menuItem.Name} - ₹{menuItem.Price}");
                if (menuItem.Recipe.Count > 0)
                {
                    Console.WriteLine("Recipe:");
                    foreach (var recipeIngredient in menuItem.Recipe)
                    {
                        Console.WriteLine($"Ingredient ID : {recipeIngredient.Key}  Quantity : {recipeIngredient.Value}");
                    }
                }
                Console.WriteLine("--------------------------------");
            }
        }

        private void DisplayBill(Bill bill)
        {
            Console.WriteLine();
            Console.WriteLine("========== BILL ==========");
            Console.WriteLine($"Order ID : {bill.OrderId}");
            Console.WriteLine($"Customer Name : {bill.CustomerName}");
            if (bill.TableNumber > 0)
            {
                Console.WriteLine($"Table Number : {bill.TableNumber}");
            }
            Console.WriteLine("--------------------------");
            if (bill.SubOrders.Count > 0)
            {
                foreach (SubOrder subOrder in bill.SubOrders)
                {
                    DisplaySubOrder(subOrder);
                    Console.WriteLine("--------------------------");
                }
            }
            else
            {
                foreach (OrderItem orderItem in bill.Items)
                {
                    Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{billManager.CalculateItemTotalPrice(orderItem)}");
                }
                Console.WriteLine("--------------------------");
            }
            Console.WriteLine($"Subtotal : ₹{bill.Subtotal}");
            Console.WriteLine($"GST (18%): ₹{bill.Tax}");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Grand Total : ₹{bill.GrandTotal}");
            Console.WriteLine(bill.SaveMessage);
        }

        private void DisplaySubOrder(SubOrder subOrder)
        {
            Console.WriteLine(GetSubOrderHeading(subOrder));
            foreach (OrderItem orderItem in subOrder.Items)
            {
                Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{billManager.CalculateItemTotalPrice(orderItem)}");
            }
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
