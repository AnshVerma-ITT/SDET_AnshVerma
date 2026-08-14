using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class InventoryManagerTests : FileTestBase
    {
        [Test]
        public void AddIngredient_WhenValid_AddsAndLoadsIngredient()
        {
            InventoryManager manager = new InventoryManager();
            Ingredient ingredient = new Ingredient(1, "Ingredient", 10, "kg");

            bool added = manager.AddIngredient(
                ingredient,
                out string message);
            Assert.That(added, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(manager.SearchIngredientByName(" ingredient "), Is.Not.Null);
            InventoryManager reloadedManager = new InventoryManager();
            Ingredient? savedIngredient = reloadedManager.SearchIngredientById(ingredient.IngredientId);
            Assert.That(savedIngredient, Is.Not.Null);
            Assert.That(savedIngredient!.Name, Is.EqualTo(ingredient.Name));
            Assert.That(savedIngredient.Quantity, Is.EqualTo(ingredient.Quantity));
            Assert.That(savedIngredient.Unit, Is.EqualTo(ingredient.Unit));
        }

        [Test]
        public void AddIngredient_WhenNameAlreadyExists_ReturnsFalse()
        {
            InventoryManager manager = new InventoryManager();
            Ingredient ingredient = new Ingredient(1, "Ingredient", 10, "kg");
            manager.AddIngredient(ingredient, out _);
            bool added = manager.AddIngredient(
                new Ingredient(2, $" {ingredient.Name.ToLower()} ", 5, "kg"),
                out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void AddIngredient_WhenQuantityIsNegative_ReturnsFalse()
        {
            InventoryManager manager = new InventoryManager();
            Ingredient ingredient = new Ingredient(1, "Ingredient", -1, "kg");
            bool added = manager.AddIngredient(ingredient, out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void UpdateIngredientQuantityByName_WhenIngredientExists_IncreasesQuantity()
        {
            InventoryManager manager = new InventoryManager();
            double startingQuantity = 4;
            double addedQuantity = 3;
            Ingredient ingredient = new Ingredient(1, "Ingredient", startingQuantity, "kg");
            manager.AddIngredient(ingredient, out _);
            bool updated = manager.UpdateIngredientQuantityByName(
                ingredient.Name.ToLower(),
                addedQuantity,
                out string message);
            Assert.That(updated, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(
                manager.SearchIngredientByName(ingredient.Name)!.Quantity,
                Is.EqualTo(startingQuantity + addedQuantity));
        }

        [Test]
        public void DeleteIngredientByName_WhenIngredientExists_RemovesIngredient()
        {
            InventoryManager manager = new InventoryManager();
            Ingredient ingredient = new Ingredient(1, "Ingredient", 4, "kg");
            manager.AddIngredient(ingredient, out _);
            bool deleted = manager.DeleteIngredientByName(ingredient.Name.ToLower(), out string message);
            Assert.That(deleted, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(manager.SearchIngredientByName(ingredient.Name), Is.Null);
        }

        [Test]
        public void CalculateRequiredIngredients_WhenItemsHaveRecipes_CombinesQuantities()
        {
            InventoryManager manager = new InventoryManager();
            int firstIngredientId = 1;
            int secondIngredientId = 2;
            double firstRequiredQuantity = 2;
            double secondRequiredQuantity = 0.5;
            int itemQuantity = 3;
            MenuItem menuItem = new MenuItem(
                1,
                "Menu Item",
                250,
                new Dictionary<int, double>
                {
                    { firstIngredientId, firstRequiredQuantity },
                    { secondIngredientId, secondRequiredQuantity }
                });
            Dictionary<int, double> requiredIngredients = manager.CalculateRequiredIngredients(
                new List<OrderItem> { new OrderItem(menuItem, itemQuantity) });
            Assert.That(
                requiredIngredients[firstIngredientId],
                Is.EqualTo(firstRequiredQuantity * itemQuantity));
            Assert.That(
                requiredIngredients[secondIngredientId],
                Is.EqualTo(secondRequiredQuantity * itemQuantity));
        }

        [Test]
        public void UseIngredients_WhenEnoughStock_ReducesQuantity()
        {
            InventoryManager manager = new InventoryManager();
            int firstIngredientId = 1;
            int secondIngredientId = 2;
            double firstStartingQuantity = 10;
            double secondStartingQuantity = 5;
            double firstUsedQuantity = 6;
            double secondUsedQuantity = 1.5;
            manager.AddIngredient(new Ingredient(firstIngredientId, "First Ingredient", firstStartingQuantity, "kg"), out _);
            manager.AddIngredient(new Ingredient(secondIngredientId, "Second Ingredient", secondStartingQuantity, "kg"), out _);

            bool used = manager.UseIngredients(
                new Dictionary<int, double>
                {
                    { firstIngredientId, firstUsedQuantity },
                    { secondIngredientId, secondUsedQuantity }
                },
                out string message);
            Assert.That(used, Is.True);
            Assert.That(message, Is.Empty);
            Assert.That(
                manager.SearchIngredientById(firstIngredientId)!.Quantity,
                Is.EqualTo(firstStartingQuantity - firstUsedQuantity));
            Assert.That(
                manager.SearchIngredientById(secondIngredientId)!.Quantity,
                Is.EqualTo(secondStartingQuantity - secondUsedQuantity));
        }

        [Test]
        public void HasEnoughIngredients_WhenStockIsLow_ReturnsFalse()
        {
            InventoryManager manager = new InventoryManager();
            Ingredient ingredient = new Ingredient(1, "Ingredient", 2, "kg");
            manager.AddIngredient(ingredient, out _);

            bool hasEnough = manager.HasEnoughIngredients(
                new Dictionary<int, double> { { ingredient.IngredientId, ingredient.Quantity + 1 } },
                out string message);
            Assert.That(hasEnough, Is.False);
            Assert.That(message, Is.Not.Empty);
        }
    }
}
