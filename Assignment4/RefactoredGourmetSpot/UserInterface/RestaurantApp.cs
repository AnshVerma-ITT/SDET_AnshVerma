using GourmetSpot.Services;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class RestaurantApp
    {
        private InventoryScreen inventoryScreen;
        private MenuScreen menuScreen;
        private OrderScreen orderScreen;
        private ReservationScreen reservationScreen;

        public RestaurantApp()
        {
            if (!FileManager.EnsureApplicationDirectoriesExist(out string folderMessage))
            {
                Console.WriteLine(folderMessage);
            }
            IInventoryManager inventoryManager = new InventoryManager();
            IMenuManager menuManager = new MenuManager();
            IOrderManager orderManager = new OrderManager();
            IBillManager billManager = new BillManager();
            IReservationManager reservationManager = new ReservationManager();
            DisplayStartupMessage(inventoryManager.LoadMessage);
            DisplayStartupMessage(menuManager.LoadMessage);
            DisplayStartupMessage(orderManager.LoadMessage);
            DisplayStartupMessage(reservationManager.LoadMessage);
            inventoryScreen = new InventoryScreen(inventoryManager);
            menuScreen = new MenuScreen(menuManager, inventoryManager);
            orderScreen = new OrderScreen(orderManager, menuManager, inventoryManager, billManager);
            reservationScreen = new ReservationScreen(reservationManager);
        }

        public void Run()
        {
            while (true)
            {
                DisplayMainMenu();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        inventoryScreen.Show();
                        break;
                    case "2":
                        menuScreen.Show();
                        break;
                    case "3":
                        orderScreen.Show();
                        break;
                    case "4":
                        reservationScreen.Show();
                        break;
                    case "5":
                        Console.WriteLine("Thank you for using the Restaurant Management System.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("====== The Gourmet Spot Restaurant Management ======");
            Console.WriteLine();
            Console.WriteLine("1. Inventory Management");
            Console.WriteLine("2. Menu Management");
            Console.WriteLine("3. Order Management");
            Console.WriteLine("4. Reservation Management");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");
        }

        private void DisplayStartupMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
        }
    }
}
