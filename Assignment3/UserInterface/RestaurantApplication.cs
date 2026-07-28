using GourmetSpot.Services;

namespace GourmetSpot.UserInterface
{
    public class RestaurantApplication
    {
        private readonly InventoryConsoleMenu inventoryConsoleMenu;
        private readonly MenuManagementConsoleMenu menuManagementConsoleMenu;
        private readonly OrderConsoleMenu orderConsoleMenu;
        private readonly ReservationConsoleMenu reservationConsoleMenu;

        public RestaurantApplication()
        {
            ApplicationStorage.EnsureApplicationDirectoriesExist();

            InventoryManager inventoryManager = new InventoryManager();
            MenuManager menuManager = new MenuManager();
            OrderManager orderManager = new OrderManager();
            BillManager billManager = new BillManager();
            ReservationManager reservationManager = new ReservationManager();

            inventoryConsoleMenu = new InventoryConsoleMenu(inventoryManager);
            menuManagementConsoleMenu = new MenuManagementConsoleMenu(menuManager, inventoryManager);
            orderConsoleMenu = new OrderConsoleMenu(orderManager, menuManager, inventoryManager, billManager);
            reservationConsoleMenu = new ReservationConsoleMenu(reservationManager);
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
                        inventoryConsoleMenu.Show();
                        break;
                    case "2":
                        menuManagementConsoleMenu.Show();
                        break;
                    case "3":
                        orderConsoleMenu.Show();
                        break;
                    case "4":
                        reservationConsoleMenu.Show();
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
    }
}
