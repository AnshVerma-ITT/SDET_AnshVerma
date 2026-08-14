using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class MenuManagerTests : FileTestBase
    {
        [Test]
        public void AddMenuItem_WhenValid_AddsAndLoadsMenuItem()
        {
            MenuManager manager = new MenuManager();
            int ingredientId = 1;
            double firstRequiredQuantity = 2;
            double secondRequiredQuantity = 3;
            double expectedRequiredQuantity = firstRequiredQuantity + secondRequiredQuantity;
            Dictionary<int, double> recipe = manager.CreateRecipe();
            manager.AddRecipeIngredient(recipe, ingredientId, firstRequiredQuantity);
            manager.AddRecipeIngredient(recipe, ingredientId, secondRequiredQuantity);
            MenuItem menuItem = new MenuItem(1, "Menu Item", 180, recipe);

            bool added = manager.AddMenuItem(
                menuItem,
                out string message);
            Assert.That(added, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(manager.SearchMenuItemByName($" {menuItem.Name.ToLower()} "), Is.Not.Null);
            Assert.That(recipe[ingredientId], Is.EqualTo(expectedRequiredQuantity));
            MenuManager reloadedManager = new MenuManager();
            MenuItem? savedMenuItem = reloadedManager.SearchMenuItemById(menuItem.MenuItemId);
            Assert.That(savedMenuItem, Is.Not.Null);
            Assert.That(savedMenuItem!.Name, Is.EqualTo(menuItem.Name));
            Assert.That(savedMenuItem.Price, Is.EqualTo(menuItem.Price));
            Assert.That(savedMenuItem.Recipe[ingredientId], Is.EqualTo(expectedRequiredQuantity));
        }

        [Test]
        public void AddMenuItem_WhenNameIsEmpty_ReturnsFalse()
        {
            MenuManager manager = new MenuManager();

            bool added = manager.AddMenuItem(
                new MenuItem(1, "", 100, new Dictionary<int, double>()),
                out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void AddMenuItem_WhenPriceIsZero_ReturnsFalse()
        {
            MenuManager manager = new MenuManager();

            bool added = manager.AddMenuItem(
                new MenuItem(1, "Menu Item", 0, new Dictionary<int, double>()),
                out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }
    }
}
