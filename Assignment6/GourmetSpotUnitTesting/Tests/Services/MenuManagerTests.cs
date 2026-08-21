using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class MenuManagerTests
    {
        [Test]
        public void GetNextMenuItemId_WhenMenuItemsExist_ReturnsCountPlusOne()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            List<MenuItem> savedMenuItems = new List<MenuItem>
            {
                TestData.CreateMenuItem()
            };
            TestDataSetter.SetField(
                manager,
                "menuItems",
                savedMenuItems);
            int nextId = manager.GetNextMenuItemId();
            Assert.That(nextId, Is.EqualTo(savedMenuItems.Count + TestData.FirstId), "GetNextMenuItemId should return current menu item count plus one.");
        }

        [Test]
        public void CreateRecipe_WhenCalled_ReturnsEmptyDictionary()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            Dictionary<int, double> recipe = manager.CreateRecipe();
            Assert.That(recipe, Is.Empty, "CreateRecipe should return an empty recipe dictionary.");
        }

        [Test]
        public void AddRecipeIngredient_WhenIngredientIsNew_AddsIngredient()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            Dictionary<int, double> recipe = new Dictionary<int, double>();
            manager.AddRecipeIngredient(
                recipe,
                TestData.FirstId,
                TestData.RequiredQuantity);
            Assert.That(recipe[TestData.FirstId], Is.EqualTo(TestData.RequiredQuantity), "AddRecipeIngredient should add a new ingredient quantity to the recipe.");
        }

        [Test]
        public void AddRecipeIngredient_WhenIngredientAlreadyExists_AddsQuantity()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            Dictionary<int, double> recipe = new Dictionary<int, double>
            {
                { TestData.FirstId, TestData.RequiredQuantity }
            };
            manager.AddRecipeIngredient(
                recipe,
                TestData.FirstId,
                TestData.ExtraRequiredQuantity);
            Assert.That(
                recipe[TestData.FirstId],
                Is.EqualTo(TestData.RequiredQuantity + TestData.ExtraRequiredQuantity),
                "AddRecipeIngredient should add quantity when the ingredient already exists in the recipe.");
        }

        [Test]
        public void SearchMenuItemById_WhenIdExists_ReturnsMenuItem()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            MenuItem menuItem = TestData.CreateMenuItem();
            TestDataSetter.SetField(
                manager,
                "menuItems",
                new List<MenuItem> { menuItem });
            MenuItem? foundMenuItem = manager.SearchMenuItemById(menuItem.MenuItemId);
            Assert.That(foundMenuItem, Is.SameAs(menuItem), "SearchMenuItemById should return the menu item with the matching id.");
        }

        [Test]
        public void SearchMenuItemByName_WhenNameMatchesIgnoringCase_ReturnsMenuItem()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            MenuItem menuItem = TestData.CreateMenuItem();
            TestDataSetter.SetField(
                manager,
                "menuItems",
                new List<MenuItem> { menuItem });
            MenuItem? foundMenuItem = manager.SearchMenuItemByName(
                TestData.DifferentCaseWithSpaces(menuItem.Name));
            Assert.That(foundMenuItem, Is.SameAs(menuItem), "SearchMenuItemByName should match menu item names ignoring case and spaces.");
        }

        [Test]
        public void GetAllMenuItems_WhenMenuItemsExist_ReturnsCopy()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            MenuItem menuItem = TestData.CreateMenuItem();
            List<MenuItem> savedMenuItems = new List<MenuItem> { menuItem };
            TestDataSetter.SetField(
                manager,
                "menuItems",
                savedMenuItems);
            List<MenuItem> menuItems = manager.GetAllMenuItems();
            Assert.That(menuItems, Is.Not.SameAs(savedMenuItems), "GetAllMenuItems should return a new list instead of the internal list.");
            Assert.That(menuItems, Has.Count.EqualTo(savedMenuItems.Count), "GetAllMenuItems should include all saved menu items.");
            Assert.That(menuItems[TestData.FirstIndex], Is.SameAs(menuItem), "GetAllMenuItems should return the saved menu item.");
        }

        [Test]
        public void AddMenuItem_WhenNameIsEmpty_ReturnsFalse()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            bool added = manager.AddMenuItem(
                TestData.CreateMenuItem(name: TestData.EmptyName),
                out string message);
            Assert.That(added, Is.False, "AddMenuItem should reject an empty menu item name.");
            Assert.That(message, Is.Not.Empty, "AddMenuItem should return a validation message for empty name.");
        }

        [Test]
        public void AddMenuItem_WhenNameIsWhiteSpace_ReturnsFalse()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            bool added = manager.AddMenuItem(
                TestData.CreateMenuItem(name: TestData.WhiteSpaceName),
                out string message);
            Assert.That(added, Is.False, "AddMenuItem should reject a whitespace menu item name.");
            Assert.That(message, Is.Not.Empty, "AddMenuItem should return a validation message for whitespace name.");
        }

        [Test]
        public void AddMenuItem_WhenPriceIsZero_ReturnsFalse()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            bool added = manager.AddMenuItem(
                TestData.CreateMenuItem(price: TestData.ZeroPrice),
                out string message);
            Assert.That(added, Is.False, "AddMenuItem should reject zero price.");
            Assert.That(message, Is.Not.Empty, "AddMenuItem should return a validation message for zero price.");
        }

        [Test]
        public void AddMenuItem_WhenRecipeIsNull_ReturnsFalse()
        {
            MenuManager manager = TestDataSetter.CreateWithoutConstructor<MenuManager>();
            bool added = manager.AddMenuItem(
                TestData.CreateMenuItemWithNullRecipe(),
                out string message);
            Assert.That(added, Is.False, "AddMenuItem should reject a null recipe.");
            Assert.That(message, Is.Not.Empty, "AddMenuItem should return a validation message for null recipe.");
        }
    }
}
