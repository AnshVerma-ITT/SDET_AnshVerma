using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class InventoryManagerTests : ManagerTestBase<InventoryManager>
    {
        [Test]
        public void GetNextIngredientId_WhenIngredientsExist_ReturnsCountPlusOne()
        {
            Console.WriteLine(
    $"{TestContext.CurrentContext.Test.Name} | Thread: {Environment.CurrentManagedThreadId}");
            List<Ingredient> savedIngredients = new List<Ingredient>
            {
                TestData.CreateIngredient()
            };
            SetField(
                "ingredients",
                savedIngredients);
            int nextId = Manager.GetNextIngredientId();
            Assert.That(nextId, Is.EqualTo(savedIngredients.Count + TestData.FirstId), "GetNextIngredientId should return current ingredient count plus one.");
        }

        [Test]
        public void GetAllIngredients_WhenIngredientsExist_ReturnsCopy()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            List<Ingredient> savedIngredients = new List<Ingredient> { ingredient };
            SetField(
                "ingredients",
                savedIngredients);
            List<Ingredient> ingredients = Manager.GetAllIngredients();
            Assert.That(ingredients, Is.Not.SameAs(savedIngredients), "GetAllIngredients should return a new list instead of the internal list.");
            Assert.That(ingredients, Has.Count.EqualTo(savedIngredients.Count), "GetAllIngredients should include all saved ingredients.");
            Assert.That(ingredients[TestData.FirstIndex], Is.SameAs(ingredient), "GetAllIngredients should return the saved ingredient item.");
        }

        [Test]
        public void AddIngredient_WhenNameAlreadyExists_ReturnsFalse()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            bool added = Manager.AddIngredient(
                TestData.CreateIngredient(
                    ingredientId: TestData.SecondId,
                    name: TestData.DifferentCaseWithSpaces(ingredient.Name),
                    quantity: TestData.StockQuantity),
                out string message);
            Assert.That(added, Is.False, "AddIngredient should reject duplicate ingredient names ignoring case and spaces.");
            Assert.That(message, Is.Not.Empty, "AddIngredient should return a validation message for duplicate ingredient names.");
        }

        [Test]
        public void AddIngredient_WhenQuantityIsNegative_ReturnsFalse()
        {
            Ingredient ingredient = TestData.CreateIngredient(quantity: TestData.NegativeQuantity);
            bool added = Manager.AddIngredient(ingredient, out string message);
            Assert.That(added, Is.False, "AddIngredient should reject negative ingredient quantity.");
            Assert.That(message, Is.Not.Empty, "AddIngredient should return a validation message for negative quantity.");
        }

        [Test]
        public void SearchIngredientById_WhenIdExists_ReturnsIngredient()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            Ingredient? foundIngredient = Manager.SearchIngredientById(ingredient.IngredientId);
            Assert.That(foundIngredient, Is.SameAs(ingredient), "SearchIngredientById should return the ingredient with the matching id.");
        }

        [Test]
        public void SearchIngredientByName_WhenNameMatchesIgnoringCase_ReturnsIngredient()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            Ingredient? foundIngredient = Manager.SearchIngredientByName(
                TestData.DifferentCaseWithSpaces(ingredient.Name));
            Assert.That(foundIngredient, Is.SameAs(ingredient), "SearchIngredientByName should match ingredient names ignoring case and spaces.");
        }

        [Test]
        public void UpdateIngredientQuantityByName_WhenQuantityIsNegative_ReturnsFalse()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            bool updated = Manager.UpdateIngredientQuantityByName(
                TestData.DifferentCaseWithSpaces(ingredient.Name),
                TestData.NegativeQuantity,
                out string message);
            Assert.That(updated, Is.False, "UpdateIngredientQuantityByName should reject negative quantity updates.");
            Assert.That(message, Is.Not.Empty, "UpdateIngredientQuantityByName should return a validation message for negative quantity.");
        }

        [Test]
        public void DeleteIngredientByName_WhenIngredientDoesNotExist_ReturnsFalse()
        {
            SetField(
                "ingredients",
                new List<Ingredient>());
            bool deleted = Manager.DeleteIngredientByName(TestData.MissingName, out string message);
            Assert.That(deleted, Is.False, "DeleteIngredientByName should return false when the ingredient does not exist.");
            Assert.That(message, Is.Not.Empty, "DeleteIngredientByName should return a message when the ingredient is missing.");
        }

        [Test]
        public void CalculateRequiredIngredients_WhenItemsHaveRecipes_CombinesQuantities()
        {
            int itemQuantity = TestData.OrderQuantity;
            MenuItem menuItem = TestData.CreateMenuItem(
                recipe:
                new Dictionary<int, double>
                {
                    { TestData.FirstId, TestData.RequiredQuantity },
                    { TestData.SecondId, TestData.SecondRequiredQuantity }
                });
            Dictionary<int, double> requiredIngredients = Manager.CalculateRequiredIngredients(
                new List<OrderItem> { TestData.CreateOrderItem(menuItem, itemQuantity) });
            Assert.That(
                requiredIngredients[TestData.FirstId],
                Is.EqualTo(TestData.RequiredQuantity * itemQuantity),
                "CalculateRequiredIngredients should multiply first recipe ingredient quantity by order item quantity.");
            Assert.That(
                requiredIngredients[TestData.SecondId],
                Is.EqualTo(TestData.SecondRequiredQuantity * itemQuantity),
                "CalculateRequiredIngredients should multiply second recipe ingredient quantity by order item quantity.");
        }

        [Test]
        public void HasEnoughIngredients_WhenStockIsAvailable_ReturnsTrue()
        {
            Ingredient ingredient = TestData.CreateIngredient();
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            bool hasEnough = Manager.HasEnoughIngredients(
                new Dictionary<int, double> { { ingredient.IngredientId, ingredient.Quantity } },
                out string message);
            Assert.That(hasEnough, Is.True, "HasEnoughIngredients should return true when stock exactly meets required quantity.");
            Assert.That(message, Is.Empty, "HasEnoughIngredients should not return an error message when stock is available.");
        }

        [Test]
        public void HasEnoughIngredients_WhenStockIsLow_ReturnsFalse()
        {
            Ingredient ingredient = TestData.CreateIngredient(quantity: TestData.LowStockQuantity);
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            bool hasEnough = Manager.HasEnoughIngredients(
                new Dictionary<int, double>
                {
                    { ingredient.IngredientId, ingredient.Quantity + TestData.SingleQuantity }
                },
                out string message);
            Assert.That(hasEnough, Is.False, "HasEnoughIngredients should return false when required quantity is greater than stock.");
            Assert.That(message, Is.Not.Empty, "HasEnoughIngredients should return a message when stock is insufficient.");
        }

        [Test]
        public void UseIngredients_WhenStockIsLow_ReturnsFalse()
        {
            Ingredient ingredient = TestData.CreateIngredient(quantity: TestData.LowStockQuantity);
            SetField(
                "ingredients",
                new List<Ingredient> { ingredient });
            bool used = Manager.UseIngredients(
                new Dictionary<int, double>
                {
                    { ingredient.IngredientId, ingredient.Quantity + TestData.SingleQuantity }
                },
                out string message);
            Assert.That(used, Is.False, "UseIngredients should return false when stock is insufficient.");
            Assert.That(message, Is.Not.Empty, "UseIngredients should return a message when stock cannot be used.");
        }
    }
}
