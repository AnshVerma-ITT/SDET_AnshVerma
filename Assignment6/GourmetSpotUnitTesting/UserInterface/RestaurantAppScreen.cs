using GourmetSpot.Services;
using GourmetSpot.Services.Contracts;
using GourmetSpot.UserInterface.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class RestaurantAppScreen : IDisplay
    {
        private readonly IDisplay inventoryScreen;
        private readonly IDisplay menuScreen;
        private readonly IDisplay orderScreen;
        private readonly IDisplay reservationScreen;

        public RestaurantAppScreen()
        {
            if (!FileManager.EnsureApplicationDirectoriesExist(out string folderMessage))
            {
                Console.WriteLine(folderMessage);
            }
            IInventoryManager inventoryManager = new InventoryManager();
            MenuManager menuManager = new MenuManager();
            OrderManager orderManager = new OrderManager();
            IBillManager billManager = new BillManager();
            ReservationManager reservationManager = new ReservationManager();
            DisplayStartupMessage(inventoryManager.LoadMessage);
            DisplayStartupMessage(menuManager.LoadMessage);
            DisplayStartupMessage(orderManager.LoadMessage);
            DisplayStartupMessage(reservationManager.LoadMessage);
            inventoryScreen = new InventoryScreen(inventoryManager);
            menuScreen = new MenuScreen(menuManager, inventoryManager);
            orderScreen = new OrderScreen(orderManager, menuManager, inventoryManager, billManager);
            reservationScreen = new ReservationScreen(reservationManager);
        }

        public void Display()
        {
            while (true)
            {
                DisplayMenu();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        inventoryScreen.Display();
                        break;
                    case "2":
                        menuScreen.Display();
                        break;
                    case "3":
                        orderScreen.Display();
                        break;
                    case "4":
                        reservationScreen.Display();
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

        public void DisplayMenu()
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
