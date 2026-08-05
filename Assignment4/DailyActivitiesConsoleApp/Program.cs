using DailyActivityTracker.Services;
using DailyActivityTracker.UserInterface;

namespace DailyActivityTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ITaskService taskService = new TaskService();
            ConsoleMenu consoleMenu = new ConsoleMenu(taskService);
            consoleMenu.Start();
        }
    }
}
