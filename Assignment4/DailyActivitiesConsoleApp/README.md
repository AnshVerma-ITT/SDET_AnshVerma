# Daily Task Manager

## Project Details

| Field | Value |
| --- | --- |
| Project Name | Daily Task Manager |
| Application Type | Console Application |
| Developed By | Ansh Verma |
| Language | C# |
| Platform | .NET Console Application |

## Project Description

The Daily Task Manager is a console-based application developed in C#. It helps users manage daily activities by adding tasks, viewing them, marking them as completed, and deleting them. The application uses Object-Oriented Programming principles and follows a clean separation of responsibilities across model, service, UI, and utility classes.

The project is structured so that the main program remains simple, while each responsibility is handled by its own class. Task data is stored in a JSON file so that information remains available between runs.

## Features Implemented

### 1. Task Management

- Add Task
- View All Tasks
- View Pending Tasks
- View Completed Tasks
- Mark Task as Completed
- Delete Task
- Automatic Task ID Creation

### 2. Data Persistence

- Save tasks to a JSON file
- Load tasks from a JSON file
- Keep task information between program runs

### 3. Validation

- Validate task title, category, and time input
- Validate task ID when performing task operations

## OOP Concepts Used

- Classes and Objects
- Constructors
- Fields
- Encapsulation
- Collections
  - `List<T>`
- Methods
- Loops
- Conditional Statements
- Exception Handling
  - `try-catch`
- File Handling
- JSON Serialization and Deserialization
- Namespaces
- Separation of Responsibilities

## Folder Structure

- `Model`: Contains the `DailyTask` class with task data and constructors.
- `Services`: Contains task business logic such as add, view, complete, and delete operations.
- `UserInterface`: Contains the console menu and display logic.
- `Utilities`: Contains input helpers, validation, and JSON file handling classes.
- `Data`: Contains `tasks.json`, where task data is stored.

## How To Run

1. Open the project folder in a terminal.
2. Run the following command:

```bash
dotnet run
```

3. Use the menu options to add, view, complete, or delete tasks.

## Expected Output

- Tasks can be added and viewed in the console.
- Pending and completed tasks can be filtered.
- Tasks can be marked as completed or deleted.
- Data is saved automatically to JSON so it remains available after the program closes.

## Conclusion

This project demonstrates the implementation of a simple daily task manager using C#. It showcases core Object-Oriented Programming concepts, console-based interaction, validation, exception handling, and JSON file persistence in a beginner-friendly way.

