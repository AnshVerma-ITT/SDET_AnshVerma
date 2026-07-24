using System;
using System.Collections.Generic;

namespace GourmetSpot.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Dictionary<int, double> Recipe { get; set; }
        public MenuItem(int menuItemId,string name,decimal price,Dictionary<int, double> recipe)
        {
            MenuItemId = menuItemId;
            Name = name;
            Price = price;
            Recipe = recipe;
        }

        public override string ToString()
        {
            return $"{MenuItemId} - {Name} - ₹{Price}";
        }
    }
}