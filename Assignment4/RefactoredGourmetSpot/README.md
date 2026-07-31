# The Gourmet Spot Restaurant Management System

## Project Details

| Field | Value |
| --- | --- |
| Project Name | The Gourmet Spot Restaurant Management System |
| Application Type | Console Application |
| Developed By | Ansh Verma |
| Language | C# |
| Platform | .NET Console Application |

## Project Description

The Restaurant Management System is a console-based application developed in C#. The application helps manage restaurant operations such as inventory management, menu management, customer order processing, bill generation, table reservations, and inventory updates after each order.

The project has been developed using Object-Oriented Programming principles and various C# concepts covered during training.

The console screens are separated into the `UserInterface` folder so `Program.cs` remains small and each module has its own class.

`FileManager` creates the `Data` and `Bills` folders at startup, ensures parent folders before writing files, and centralizes file paths and file handling exception management.

## Assignment 2 Refactor Summary

The project has been refactored to address the client escalation about maintainability, scalability, and flexibility.

- **Abstraction:** Service contracts were added under `Services/Contracts` so UI classes depend on interfaces such as `IOrderManager`, `IInventoryManager`, `IMenuManager`, `IBillManager`, and `IReservationManager`.
- **Inheritance and Polymorphism:** `Order` is now an abstract base class. `CustomerOrder` and `TableOrder` extend it, allowing different order types to define their own behavior such as whether they can receive more suborders.
- **Encapsulation:** `Order` and `SubOrder` now protect their internal item collections and expose read-only views. Items are added through domain methods.
- **Single Responsibility Principle:** Order storage was moved from `OrderManager` into `TextOrderRepository`; recipe inventory calculation was moved into `RecipeInventoryRequirementCalculator`.
- **Open/Closed Principle:** New order types can be added through new `Order` subclasses and the `OrderFactory` without rewriting billing, inventory, or UI flow.
- **Dependency Inversion Principle:** Screens and services communicate through interfaces instead of direct concrete dependencies.
- **DRY:** Shared order request validation, inventory consumption, suborder creation, and order serialization are centralized.
- **Backward Compatibility:** Existing saved orders continue to load, while newly saved orders include an explicit order type for future extension.

## Features Implemented

### 1. Inventory Management

- Add Ingredient
- View Ingredients
- Search Ingredient by Name
- Update Ingredient Quantity by Name
- Delete Ingredient by Name
- Automatic Ingredient ID Creation
- Duplicate Ingredient Check
- Automatic Inventory Save and Load

### 2. Menu Management

- Add Menu Item
- View Menu Items
- Search Menu Item by Name
- Menu Recipe Management
- Automatic Menu Item ID Creation
- JSON Serialization for Menu Data

### 3. Order Management

- Create Customer Order
- Customer Name on Order
- Multiple Menu Items in One Order
- Table Order Sessions
- Multiple Suborders Linked to One Table
- Add More Items to an Active Table Order
- Finalize Table Order Before Bill Generation
- Quantity Management
- Automatic Order ID Creation
- View Orders

### 4. Billing

- Automatic Bill Generation
- Final Bill Calculation After Table Order Completion
- Bill Breakdown by Suborder for Table Orders
- GST Calculation at 18%
- Tax Rate Managed in Billing Module
- Bill Displayed in Console
- Bill Saved to `Bills` Folder using File Handling
- Customer Name and Table Number Included on Bill

### 5. Inventory Update

- Automatically reduces ingredient quantities after order placement
- Prevents order placement if stock is insufficient

### 6. Reservation Management

- Create Table Reservation
- Validate Customer Name
- Validate 10 Digit Contact Number
- Validate Reservation Date and Time
- Prevent Past Reservations
- Prevent Reservations More Than 3 Months Ahead
- 2 Hour Reservation Window
- Show Available Tables Before Booking
- View Reservations
- Search Reservation
- Cancel Reservation
- Automatic Reservation ID Creation
- Reservation Save and Load

## C# Concepts Used

- Classes and Objects
- Constructors
- Properties
- Interfaces
- Abstract Classes
- Inheritance
- Polymorphism
- Encapsulation
- Abstraction
- Object Composition
- SOLID Principles
- DRY Principle
- Dependency Injection by Constructor
- Repository Pattern for Order Storage
- Factory Pattern for Order Creation
- Collections
  - `List<T>`
  - `Dictionary<TKey, TValue>`
- Methods
- Loops
- Conditional Statements
- Exception Handling
  - `try-catch`
  - `IOException`
  - `InvalidOperationException`
  - `JsonException`
  - `Exception`
- File Handling
  - Read
  - Write
  - Directory Management
  - File Handling Exception Management
- JSON Serialization
- JSON Deserialization
- DateTime Handling
- Namespaces
- Separation of Responsibilities
- Centralized Storage Folder Management

## How To Run

1. Open the project in Visual Studio or Visual Studio Code.
2. Build the project.
3. Run the application.
4. Use the menu options to:
   - Manage Inventory
   - Manage Menu
   - Create Orders
   - Start table orders, add suborders, and finalize table bills
   - Generate Bills
   - Manage Reservations
5. Menu data is automatically stored in `Data/Menu.json`.
6. Inventory data is automatically stored in `Data/Inventory.txt`.
7. Order data is automatically stored in `Data/Orders.txt`.
8. Reservation data is automatically stored in `Data/Reservations.txt`.
9. Generated bills are automatically stored in the `Bills` folder.

## Expected Output

- Ingredients can be added and managed.
- Menu items can be created with recipes.
- Customer orders can contain multiple menu items.
- Table orders can contain multiple suborders before the final bill is generated.
- Inventory is automatically updated after successful order placement.
- Bills are generated with GST calculation, customer name, table number where available, and saved to a bill file.
- Table reservations can be created after selecting from available tables for a 2 hour time window.
- Menu information is stored using JSON Serialization.

## Conclusion

This project demonstrates the implementation of a Restaurant Management System using C#. It covers fundamental programming concepts, Object-Oriented Programming principles, collections, exception handling, file handling, and JSON serialization while providing a simple and functional console-based solution for restaurant operations.
