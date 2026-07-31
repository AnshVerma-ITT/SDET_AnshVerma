# Assignment 2 Refactor Explanation

## 1. Purpose Of This Document

This document explains, in beginner-friendly language, what changes were made in the Restaurant Management System project, why those changes were made, and where those changes were implemented in the code.

The work was done in two major stages:

1. A practical feature was added for restaurant table ordering:
   - A table can place the first order.
   - The same table can later add more suborders.
   - The final bill is generated only when the meal is complete.

2. The project was then refactored for Assignment 2:
   - The code was improved using Object-Oriented Programming concepts.
   - SOLID principles were applied.
   - Repeated code was reduced using DRY.
   - The structure was made easier to maintain and extend.

The main goal was to keep the existing functionality working while improving the internal design of the project.

---

## 2. Original Problem In The Project

Before these changes, the project worked, but the structure had some issues.

### 2.1 Order And Billing Were Too Simple

Earlier, an order was treated as one complete order immediately.

That means the flow was like this:

```text
Create order -> Add items -> Generate bill immediately
```

But in a real restaurant, customers at a table do not always order everything at once.

Example:

```text
Table 5:
1. First orders Burger and Pizza
2. Later orders Pasta
3. Later orders Dessert
4. After meal is complete, final bill is generated
```

So the system needed a way to store multiple smaller orders under one table order.

These smaller orders are called suborders.

---

### 2.2 OrderManager Had Too Many Responsibilities

Before refactoring, `OrderManager` was doing too many things:

- Creating orders
- Adding items to orders
- Updating inventory
- Calculating required ingredients
- Saving orders to file
- Loading orders from file
- Parsing saved order text
- Managing table orders

This is not good for maintainability because if one class does too many jobs, it becomes hard to change.

For example, if we wanted to change the order file format, we would have to edit `OrderManager`.
If we wanted to change inventory calculation, again we would have to edit `OrderManager`.
If we wanted to add a new order type, again `OrderManager` would need more changes.

This breaks the Single Responsibility Principle.

---

### 2.3 UI Was Depending Directly On Concrete Classes

Earlier, screens such as `OrderScreen`, `MenuScreen`, and `InventoryScreen` directly used concrete manager classes.

Example:

```csharp
private OrderManager orderManager;
private MenuManager menuManager;
```

This works, but it is less flexible.

If later we want a different implementation of order management, testing, or storage, the UI must be changed.

The better design is for UI classes to depend on interfaces.

Example:

```csharp
private IOrderManager orderManager;
private IMenuManager menuManager;
```

This follows the Dependency Inversion Principle.

---

## 3. First Feature Added: Multiple Suborders Per Table

## 3.1 What Was Added

A new concept called `SubOrder` was added.

Location:

```text
Models/SubOrder.cs
```

A `SubOrder` represents one round of ordering at a table.

Example:

```text
Table 5
  Suborder 1:
    Burger x 1
    Pizza x 1

  Suborder 2:
    Pasta x 2

  Suborder 3:
    Cold Drink x 2
```

The final bill is then calculated from all suborders together.

---

## 3.2 Why SubOrder Was Needed

Without `SubOrder`, all items were stored in one flat list.

That made it hard to know:

- Which items were ordered first
- Which items were added later
- How many times the table ordered during the meal
- How to show a proper suborder breakdown in the final bill

With `SubOrder`, the system can now store order history more realistically.

---

## 3.3 Where SubOrder Is Used

`SubOrder` is used inside the `Order` class.

Location:

```text
Models/Order.cs
```

The `Order` class now has:

```csharp
public IReadOnlyList<SubOrder> SubOrders => subOrders;
public IReadOnlyList<OrderItem> Items => items;
```

Meaning:

- `SubOrders` stores each ordering round.
- `Items` stores a flattened list of all items for billing.

This means billing can still calculate the total easily, but the system also remembers the suborder breakdown.

---

## 3.4 Table Order Menu Options Added

The order menu was updated.

Location:

```text
UserInterface/OrderScreen.cs
```

The menu now contains:

```text
1. Create Order
2. View Orders
3. Back
4. Start Table Order
5. Add Suborder To Table
6. Finalize Table Bill
```

Important point:

The old options `1`, `2`, and `3` were kept the same:

- `1` still creates a normal order
- `2` still views orders
- `3` still goes back

This was done to avoid disturbing existing functionality.

---

## 3.5 Table Order Flow

The new table order flow works like this:

```text
Start Table Order
        |
        v
Add Suborder 1
        |
        v
Table remains open
        |
        v
Add Suborder 2, 3, etc.
        |
        v
Finalize Table Bill
        |
        v
Generate one final bill
```

This matches how real restaurant table ordering works.

---

## 3.6 Important Methods For Table Ordering

Location:

```text
Services/OrderManager.cs
```

Important methods:

```csharp
StartTableOrder(...)
AddSubOrderToTable(...)
FinalizeTableOrder(...)
SearchActiveOrderByTable(...)
GetActiveTableOrders()
```

What they do:

- `StartTableOrder` creates a new open table order.
- `AddSubOrderToTable` adds more items to an existing open table order.
- `FinalizeTableOrder` marks the table order as complete.
- `SearchActiveOrderByTable` finds the current open order for a table.
- `GetActiveTableOrders` lists all currently open table orders.

---

## 3.7 Billing With Suborders

Billing was updated to show suborders separately.

Location:

```text
Services/BillManager.cs
```

Earlier bill output was like:

```text
Burger x 1
Pizza x 1
Pasta x 2
```

Now bill output can show:

```text
Suborder 1
Burger x 1
Pizza x 1

Suborder 2
Pasta x 2
```

The total is still calculated once at the end.

This makes the bill clearer and more realistic.

---

## 4. Bill File Permission Problem Fixed

## 4.1 What Problem Happened

The app showed this error:

```text
Unexpected error while writing file 'Bills/Bill_306.txt':
Access to the path is denied.
```

This happened because the `Bills` folder was read-only.

The app could read old bills, but it could not create a new bill file.

---

## 4.2 What Was Done

The folder permissions were corrected for:

```text
Data
Bills
```

The app also received a startup permission check.

Location:

```text
Utilities/FileManager.cs
```

Important method:

```csharp
EnsureDirectoryExistsAndWritable(...)
```

This method checks whether the folder exists and whether the app can write to it.

---

## 4.3 Why This Was Done

Without this check, the app could fail later while saving:

- Orders
- Inventory
- Reservations
- Bills

Now the app checks the folder permissions early, so the problem is easier to understand.

---

## 4.4 Missing Bill Was Recreated

Because order `306` was already saved but the bill file failed to write, the missing bill was recreated.

Location:

```text
Bills/Bill_306.txt
```

This file contains the final bill for order `306`.

---

## 5. Assignment 2 Refactor: Big Picture

Assignment 2 asked for urgent refactoring using:

- Object-Oriented Programming
- Polymorphism
- Inheritance
- Abstraction
- Encapsulation
- SOLID principles
- DRY principle

The project was refactored so that each part of the system has a clearer responsibility.

The main idea was:

```text
Before:
One class doing many things

After:
Multiple smaller classes, each doing one clear job
```

---

## 6. Object-Oriented Programming Concepts Applied

## 6.1 Abstraction

Abstraction means hiding implementation details and exposing only what is necessary.

Interfaces were added for services.

Location:

```text
Services/Contracts/
```

Interfaces added:

```text
IBillManager.cs
IInventoryManager.cs
IInventoryRequirementCalculator.cs
IMenuManager.cs
IOrderFactory.cs
IOrderManager.cs
IOrderRepository.cs
IReservationManager.cs
```

Example:

```csharp
public interface IOrderManager
{
    int GetNextOrderId();
    List<Order> GetAllOrders();
    bool FinalizeTableOrder(int tableNumber, out Order? order, out string message);
}
```

The UI does not need to know how orders are saved internally.
It only needs to know what operations are available.

---

## 6.2 Inheritance

Inheritance means one class can reuse and extend another class.

The base class is:

```text
Models/Order.cs
```

It is now:

```csharp
public abstract class Order
```

Two child classes were added:

```text
Models/CustomerOrder.cs
Models/TableOrder.cs
```

Relationship:

```text
Order
  |
  |-- CustomerOrder
  |
  |-- TableOrder
```

This means both `CustomerOrder` and `TableOrder` are orders, but they can behave differently.

---

## 6.3 Polymorphism

Polymorphism means the same base type can represent different child types.

Example:

```csharp
Order order = new CustomerOrder(...);
Order order = new TableOrder(...);
```

Both are `Order`, but the actual behavior can be different.

In this project:

```csharp
public abstract string OrderType { get; }
public virtual bool CanReceiveSubOrder => false;
```

In `CustomerOrder`:

```csharp
public override string OrderType => OrderTypes.Customer;
```

In `TableOrder`:

```csharp
public override string OrderType => OrderTypes.Table;
public override bool CanReceiveSubOrder => !IsFinalized;
```

This means:

- A normal customer order cannot receive more suborders.
- A table order can receive more suborders only while it is open.

This removes hard-coded checks from different places.

---

## 6.4 Encapsulation

Encapsulation means protecting internal data so it cannot be changed incorrectly from outside.

Earlier, lists could be modified more directly.

Now `Order` protects its internal lists:

Location:

```text
Models/Order.cs
```

Code idea:

```csharp
private readonly List<SubOrder> subOrders;
private readonly List<OrderItem> items;

public IReadOnlyList<SubOrder> SubOrders => subOrders;
public IReadOnlyList<OrderItem> Items => items;
```

This means other classes can read the lists but cannot directly replace or randomly modify them.

Items are added through methods:

```csharp
AddSubOrder(...)
RebuildItemsFromSubOrders()
```

Similarly, `SubOrder` now has:

```csharp
private readonly List<OrderItem> items;
public IReadOnlyList<OrderItem> Items => items;
public void AddItem(OrderItem orderItem)
```

This protects suborder data.

---

## 7. SOLID Principles Applied

## 7.1 S - Single Responsibility Principle

Single Responsibility means one class should have one main job.

### Before

`OrderManager` was doing:

- Order creation
- Inventory calculation
- File saving
- File loading
- Parsing text files
- Table order rules

### After

Responsibilities were separated:

```text
OrderManager
  Handles order workflow

TextOrderRepository
  Handles saving and loading orders from text file

RecipeInventoryRequirementCalculator
  Calculates required ingredients from recipes

OrderFactory
  Creates the correct order type
```

Locations:

```text
Services/OrderManager.cs
Services/TextOrderRepository.cs
Services/RecipeInventoryRequirementCalculator.cs
Services/OrderFactory.cs
```

Now each class has a clearer job.

---

## 7.2 O - Open/Closed Principle

Open/Closed means code should be open for extension but closed for modification.

In simple words:

You should be able to add new features without rewriting old code everywhere.

Example:

If later we want to add:

```text
DeliveryOrder
TakeawayOrder
OnlineOrder
```

We can add new classes that inherit from `Order`.

The base structure already supports multiple order types.

Location:

```text
Models/Order.cs
Models/CustomerOrder.cs
Models/TableOrder.cs
Services/OrderFactory.cs
```

---

## 7.3 L - Liskov Substitution Principle

This means child classes should be usable wherever the parent class is expected.

In this project, methods can accept:

```csharp
Order order
```

And the actual object can be:

```csharp
CustomerOrder
TableOrder
```

Example:

```csharp
Bill CreateBill(Order order)
```

Billing does not need to know whether it received a customer order or a table order.
It can work with the base `Order` type.

Location:

```text
Services/BillManager.cs
```

---

## 7.4 I - Interface Segregation Principle

This means classes should not be forced to depend on methods they do not need.

Instead of one giant interface, separate interfaces were created:

```text
IOrderManager
IMenuManager
IInventoryManager
IBillManager
IReservationManager
```

Each screen depends only on the interface it needs.

Examples:

```text
InventoryScreen uses IInventoryManager
MenuScreen uses IMenuManager and IInventoryManager
OrderScreen uses IOrderManager, IMenuManager, IInventoryManager, IBillManager
ReservationScreen uses IReservationManager
```

Locations:

```text
UserInterface/InventoryScreen.cs
UserInterface/MenuScreen.cs
UserInterface/OrderScreen.cs
UserInterface/ReservationScreen.cs
```

---

## 7.5 D - Dependency Inversion Principle

This means high-level classes should depend on abstractions, not concrete classes.

### Before

The UI depended on concrete classes:

```csharp
private OrderManager orderManager;
```

### After

The UI depends on interfaces:

```csharp
private IOrderManager orderManager;
```

This makes the project more flexible.

The concrete objects are created in one place:

```text
UserInterface/RestaurantApp.cs
```

Example:

```csharp
IOrderManager orderManager = new OrderManager();
IBillManager billManager = new BillManager();
```

This is a simple form of constructor dependency injection.

---

## 8. DRY Principle Applied

DRY means "Do Not Repeat Yourself".

Repeated logic was reduced by moving common behavior into reusable methods/classes.

Examples:

### 8.1 Order Request Validation

Location:

```text
Services/OrderManager.cs
```

Shared method:

```csharp
ValidateOrderRequest(...)
```

This prevents writing the same validation logic repeatedly in create order, start table order, and add suborder.

---

### 8.2 Inventory Requirement Calculation

Location:

```text
Services/RecipeInventoryRequirementCalculator.cs
```

Instead of calculating ingredients inside `OrderManager`, the logic is now reusable.

This class converts selected menu items into required ingredient quantities.

Example:

```text
Burger recipe:
Tomato x 1
Cheese x 1

If customer orders 3 Burgers:
Tomato required = 3
Cheese required = 3
```

---

### 8.3 Order Storage Logic

Location:

```text
Services/TextOrderRepository.cs
```

Saving and loading orders is centralized here.

This avoids spreading file parsing code across different business classes.

---

## 9. Important New Files And Their Purpose

## 9.1 Models/Order.cs

Purpose:

Defines the abstract base order.

Why:

So different order types can share common data and behavior.

Contains:

- `OrderId`
- `CustomerName`
- `IsFinalized`
- `TableNumber`
- `OrderType`
- `SubOrders`
- `Items`

---

## 9.2 Models/CustomerOrder.cs

Purpose:

Represents a normal customer order.

Why:

A normal order is completed immediately and does not receive later suborders.

---

## 9.3 Models/TableOrder.cs

Purpose:

Represents an order linked to a restaurant table.

Why:

A table order can remain open and receive multiple suborders.

Important behavior:

```csharp
public override bool CanReceiveSubOrder => !IsFinalized;
```

Meaning:

The table can receive more suborders only until the bill is finalized.

---

## 9.4 Models/SubOrder.cs

Purpose:

Represents one round of ordering inside a table order.

Why:

Customers may order multiple times before completing the meal.

---

## 9.5 Models/OrderItemSelection.cs

Purpose:

Represents a menu item selected by the user and its quantity before it becomes an actual order item.

Why:

Earlier, the code used tuple values like:

```csharp
List<(MenuItem MenuItem, int Quantity)>
```

That is harder for beginners to understand and harder to extend.

Now the code uses:

```csharp
List<OrderItemSelection>
```

This is more object-oriented.

---

## 9.6 Services/OrderManager.cs

Purpose:

Handles the order workflow.

It now focuses on:

- Creating orders
- Starting table orders
- Adding suborders
- Finalizing table orders
- Calling inventory update logic
- Calling repository save logic

It no longer handles file parsing directly.

---

## 9.7 Services/TextOrderRepository.cs

Purpose:

Handles saving and loading order data from:

```text
Data/Orders.txt
```

Why:

File handling is a storage responsibility, not an order workflow responsibility.

This improves Single Responsibility.

---

## 9.8 Services/RecipeInventoryRequirementCalculator.cs

Purpose:

Calculates how much inventory is required for selected menu items.

Why:

Inventory calculation should be separate from order placement.

This makes inventory rules easier to change later.

---

## 9.9 Services/OrderFactory.cs

Purpose:

Creates the correct type of order.

Example:

```text
If saved order type is Customer -> create CustomerOrder
If saved order type is Table -> create TableOrder
```

Why:

This avoids putting order construction logic everywhere.

---

## 9.10 Services/Contracts Folder

Purpose:

Contains interfaces.

Why:

Interfaces make the code flexible and testable.

Files:

```text
IBillManager.cs
IInventoryManager.cs
IInventoryRequirementCalculator.cs
IMenuManager.cs
IOrderFactory.cs
IOrderManager.cs
IOrderRepository.cs
IReservationManager.cs
```

---

## 10. Order Save Format Change

## 10.1 Old Format

Earlier order data was saved like:

```text
OrderId|CustomerName|Items
```

Example:

```text
304|ansh|3:Pasta:500:1
```

---

## 10.2 New Format

Now order data is saved with order type:

```text
OrderId|CustomerName|OrderType|TableNumber|Status|SubOrders
```

Example:

```text
306|ansh verma|Table|5|Finalized|1,2026-07-30T23:43:20,1:Burger:199:1
```

Meaning:

```text
306                 Order ID
ansh verma          Customer name
Table               Order type
5                   Table number
Finalized           Order status
1,...               Suborder data
```

---

## 10.3 Backward Compatibility

The project can still load older saved orders.

This was important because old data should not break after refactoring.

Location:

```text
Services/TextOrderRepository.cs
```

It supports:

- Old flat order format
- Previous suborder format
- New order type format

---

## 11. Billing Flow After Refactor

Billing works with the base `Order` class.

Location:

```text
Services/BillManager.cs
```

Flow:

```text
Order is passed to BillManager
        |
        v
Subtotal is calculated from all order items
        |
        v
GST is calculated
        |
        v
Grand total is calculated
        |
        v
Bill is saved to Bills folder
```

Because `CustomerOrder` and `TableOrder` both inherit from `Order`, billing works for both.

This is polymorphism in practice.

---

## 12. Inventory Flow After Refactor

When an order is placed:

```text
Selected menu items
        |
        v
RecipeInventoryRequirementCalculator calculates required ingredients
        |
        v
InventoryManager checks stock
        |
        v
InventoryManager reduces stock
        |
        v
Order is saved
```

Locations:

```text
Services/RecipeInventoryRequirementCalculator.cs
Services/InventoryManager.cs
Services/OrderManager.cs
```

This makes the inventory update logic easier to maintain.

---

## 13. UI Flow After Refactor

The user interface still looks almost the same to the user.

But internally, it now depends on interfaces.

Example from `OrderScreen`:

```csharp
private IOrderManager orderManager;
private IMenuManager menuManager;
private IInventoryManager inventoryManager;
private IBillManager billManager;
```

Why this is good:

- The UI does not care how orders are saved.
- The UI does not care how inventory is stored.
- The UI only calls available operations.

This makes future changes safer.

---

## 14. Why Existing Functionality Was Not Disturbed

The old features still exist:

- Add inventory
- View inventory
- Add menu items
- View menu
- Create normal customer orders
- View orders
- Generate bills
- Create reservations
- Search reservations
- Cancel reservations

The new features were added without removing old features.

Also, the old menu choices in order management were preserved:

```text
1. Create Order
2. View Orders
3. Back
```

New features were added after them:

```text
4. Start Table Order
5. Add Suborder To Table
6. Finalize Table Bill
```

This was done carefully so that users already familiar with the old app are not confused.

---

## 15. Build And Testing Done

The project was built using:

```text
dotnet build --no-restore
```

Result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

The table order flow was also tested in a temporary folder so real project data would not be accidentally changed.

Test flow:

```text
1. Start table order
2. Add first suborder
3. Add second suborder
4. Finalize table bill
5. Confirm final bill includes all suborders
```

The test passed.

---

## 16. Simple Explanation You Can Say To Teacher

You can explain the refactor like this:

```text
I refactored the Restaurant Management System to make it more maintainable and scalable.
Earlier, OrderManager had too many responsibilities. I separated order storage into TextOrderRepository, inventory calculation into RecipeInventoryRequirementCalculator, and order creation into OrderFactory.

I converted Order into an abstract base class and created CustomerOrder and TableOrder subclasses. This applies inheritance and polymorphism. TableOrder can receive multiple suborders until it is finalized, while CustomerOrder is completed directly.

I added service interfaces so the UI depends on abstractions instead of concrete classes. This applies dependency inversion and makes the project easier to extend and test.

I also improved encapsulation by protecting internal order and suborder item lists. The application still supports old features, but now it can handle table orders with multiple suborders and final billing more cleanly.
```

---

## 17. File Change Summary

### New model files

```text
Models/CustomerOrder.cs
Models/TableOrder.cs
Models/OrderTypes.cs
Models/OrderItemSelection.cs
```

### Updated model files

```text
Models/Order.cs
Models/SubOrder.cs
Models/Bill.cs
```

### New service contract files

```text
Services/Contracts/IBillManager.cs
Services/Contracts/IInventoryManager.cs
Services/Contracts/IInventoryRequirementCalculator.cs
Services/Contracts/IMenuManager.cs
Services/Contracts/IOrderFactory.cs
Services/Contracts/IOrderManager.cs
Services/Contracts/IOrderRepository.cs
Services/Contracts/IReservationManager.cs
```

### New service implementation files

```text
Services/OrderFactory.cs
Services/TextOrderRepository.cs
Services/RecipeInventoryRequirementCalculator.cs
```

### Updated service files

```text
Services/OrderManager.cs
Services/BillManager.cs
Services/InventoryManager.cs
Services/MenuManager.cs
Services/ReservationManager.cs
```

### Updated UI files

```text
UserInterface/OrderScreen.cs
UserInterface/InventoryScreen.cs
UserInterface/MenuScreen.cs
UserInterface/ReservationScreen.cs
UserInterface/RestaurantApp.cs
```

### Updated utility file

```text
Utilities/FileManager.cs
```

### Updated documentation

```text
README.md
Assignment2_Refactor_Explanation.md
```

---

## 18. Final Result

The final result is a cleaner, more object-oriented Restaurant Management System.

The project now supports:

- Normal customer orders
- Table orders
- Multiple suborders per table
- Final bill after meal completion
- Clearer billing breakdown
- Safer file permission checks
- Better separation of responsibilities
- Interface-based design
- Easier future extension

The code is now more suitable for the client escalation requirement because it is easier to maintain, easier to scale, and easier to extend.
