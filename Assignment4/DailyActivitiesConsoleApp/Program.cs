using Assignment1_OOP_DailyActivities.Services;
using Assignment1_OOP_DailyActivities.UserInterface;

namespace Assignment1_OOP_DailyActivities
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TaskService taskService = new TaskService();
            ConsoleMenu consoleMenu = new ConsoleMenu(taskService);
            consoleMenu.Start();
        }
    }
}
