using System;
using System.Collections.Generic;
using Assignment1_OOP_DailyActivities.Model;
using Assignment1_OOP_DailyActivities.Services;
using Assignment1_OOP_DailyActivities.Utilities;

namespace Assignment1_OOP_DailyActivities.UserInterface
{
    public class ConsoleMenu
    {
        private ITaskService taskService;

        public ConsoleMenu(ITaskService service)
        {
            taskService = service;
        }

        public void Start()
        {
            bool running = true;

            while (running)
            {
                try
                {
                    DisplayMenu();
                    int choice = InputHelper.ReadNumber("Enter choice: ");

                    if (choice == 1)
                    {
                        AddTask();
                    }
                    else if (choice == 2)
                    {
                        ShowTasks(taskService.GetAllTasks(), "All Daily Tasks");
                    }
                    else if (choice == 3)
                    {
                        ShowTasks(taskService.GetPendingTasks(), "Pending Tasks");
                    }
                    else if (choice == 4)
                    {
                        ShowTasks(taskService.GetCompletedTasks(), "Completed Tasks");
                    }
                    else if (choice == 5)
                    {
                        CompleteTask();
                    }
                    else if (choice == 6)
                    {
                        DeleteTask();
                    }
                    else if (choice == 7)
                    {
                        running = false;
                        Console.WriteLine("Thank you for using Daily Task Manager.");
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid menu option.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                Console.WriteLine();
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("Daily Task Manager");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View All Tasks");
            Console.WriteLine("3. View Pending Tasks");
            Console.WriteLine("4. View Completed Tasks");
            Console.WriteLine("5. Mark Task Completed");
            Console.WriteLine("6. Delete Task");
            Console.WriteLine("7. Exit");
        }

        private void AddTask()
        {
            try
            {
                string title = InputHelper.ReadText("Enter task title: ");
                string category = InputHelper.ReadText("Enter category: ");
                string time = InputHelper.ReadText("Enter time: ");

                taskService.AddTask(title, category, time);
                Console.WriteLine("Task added successfully.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CompleteTask()
        {
            try
            {
                int id = InputHelper.ReadNumber("Enter task id: ");
                taskService.MarkTaskCompleted(id);
                Console.WriteLine("Task marked as completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DeleteTask()
        {
            try
            {
                int id = InputHelper.ReadNumber("Enter task id: ");
                taskService.DeleteTask(id);
                Console.WriteLine("Task deleted successfully.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ShowTasks(List<DailyTask> dailyTasks, string heading)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine(heading);
                Console.WriteLine("---------------------------------");

                if (dailyTasks.Count == 0)
                {
                    Console.WriteLine("No tasks available.");
                    return;
                }

                for (int index = 0; index < dailyTasks.Count; index++)
                {
                    DailyTask dailyTask = dailyTasks[index];
                    Console.WriteLine("Id: " + dailyTask.Id);
                    Console.WriteLine("Title: " + dailyTask.Title);
                    Console.WriteLine("Category: " + dailyTask.Category);
                    Console.WriteLine("Time: " + dailyTask.Time);
                    Console.WriteLine("Status: " + dailyTask.GetStatus());
                    Console.WriteLine("---------------------------------");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
