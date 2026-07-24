===========================================================
      THE GOURMET SPOT RESTAURANT MANAGEMENT SYSTEM      
===========================================================

Project Name:
The Gourmet Spot Restaurant Management System (Console Application)

Developed By:
Ansh Verma

Language:
C#

Platform:
.NET Console Application

==========================================
PROJECT DESCRIPTION
==========================================

The Restaurant Management System is a console-based application developed in C#. The application helps manage restaurant operations such as inventory management, menu management, customer order processing, bill generation, and inventory updates after each order.

The project has been developed using Object-Oriented Programming principles and various C# concepts covered during training.

==========================================
FEATURES IMPLEMENTED
==========================================

1. Inventory Management
   - Add Ingredient
   - View Ingredients
   - Search Ingredient
   - Update Ingredient Quantity
   - Delete Ingredient
   - Automatic Inventory Save & Load

2. Menu Management
   - Add Menu Item
   - View Menu Items
   - Search Menu Item
   - Menu Recipe Management
   - JSON Serialization for Menu Data

3. Order Management
   - Create Customer Order
   - Multiple Menu Items in One Order
   - Quantity Management
   - View Orders

4. Billing
   - Automatic Bill Generation
   - GST Calculation (18%)
   - Bill Saved to File

5. Inventory Update
   - Automatically reduces ingredient quantities after order placement
   - Prevents order placement if stock is insufficient

==========================================
C# CONCEPTS USED
==========================================

• Classes and Objects
• Constructors
• Properties
• Encapsulation
• Object Composition
• Collections
    - List<T>
    - Dictionary<TKey, TValue>
• Methods
• Loops
• Conditional Statements
• Exception Handling
    - try-catch
    - ArgumentException
    - FormatException
• File Handling
    - Read
    - Write
    - Directory Management
• JSON Serialization
• JSON Deserialization
• Method Overriding (ToString)
• Namespaces

==========================================
HOW TO RUN
==========================================

1. Open the project in Visual Studio or Visual Studio Code.
2. Build the project.
3. Run the application.
4. Use the menu options to:
   - Manage Inventory
   - Manage Menu
   - Create Orders
   - Generate Bills
5. Menu data is automatically stored in menu.json.
6. Inventory and bills are automatically saved in the Data folder.

==========================================
EXPECTED OUTPUT
==========================================

✔ Ingredients can be added and managed.

✔ Menu items can be created with recipes.

✔ Customer orders can contain multiple menu items.

✔ Inventory is automatically updated after successful order placement.

✔ Bills are generated with GST calculation.

✔ Menu information is stored using JSON Serialization.

==========================================
CONCLUSION
==========================================

This project demonstrates the implementation of a Restaurant Management System using C#. It covers fundamental programming concepts, Object-Oriented Programming principles, collections, exception handling, file handling, and JSON serialization while providing a simple and functional console-based solution for restaurant operations.