using System.Text.Json;
using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class MenuManager
    {
        private readonly List<MenuItem> menu;

        private readonly string filePath = "Data/menu.json";

        public MenuManager()
        {
            menu = new List<MenuItem>();

            Directory.CreateDirectory("Data");

            LoadMenu();
        }

        public void AddMenuItem(MenuItem item)
        {
            menu.Add(item);

            SaveMenu();

            Console.WriteLine("Menu Item Added Successfully.");
        }

        public void DisplayMenu()
        {
            if (menu.Count == 0)
            {
                Console.WriteLine("Menu is Empty.");
                return;
            }

            Console.WriteLine("\n========== MENU ==========");

            foreach (MenuItem item in menu)
            {
                Console.WriteLine(item);

                if (item.Recipe.Count > 0)
                {
                    Console.WriteLine("Recipe:");

                    foreach (var ingredient in item.Recipe)
                    {
                        Console.WriteLine($"Ingredient ID : {ingredient.Key}  Quantity : {ingredient.Value}");
                    }
                }

                Console.WriteLine("--------------------------------");
            }
        }

        public MenuItem? SearchMenuItem(int id)
        {
            foreach (MenuItem item in menu)
            {
                if (item.MenuItemId == id)
                    return item;
            }

            return null;
        }

        private void SaveMenu()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(menu, options);

            File.WriteAllText(filePath, json);
        }

        private void LoadMenu()
        {
            if (!File.Exists(filePath))
                return;

            string json = File.ReadAllText(filePath);

            List<MenuItem>? loadedMenu =
                JsonSerializer.Deserialize<List<MenuItem>>(json);

            if (loadedMenu != null)
            {
                menu.AddRange(loadedMenu);
            }
        }

        public List<MenuItem> GetAllMenuItems()
        {
            return menu;
        }
    }
}