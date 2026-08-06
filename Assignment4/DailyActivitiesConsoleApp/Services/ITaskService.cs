using System.Collections.Generic;
using DailyActivityTracker.Model;

namespace DailyActivityTracker.Services
{
    public interface ITaskService
    {
        void AddTask(string title, string category, string time);
        List<DailyTask> GetAllTasks();
        DailyTask GetTaskById(int id);
        void MarkTaskCompleted(int id);
        void DeleteTask(int id);
        List<DailyTask> GetPendingTasks();
        List<DailyTask> GetCompletedTasks();
        string GetTaskStatus(DailyTask task);
    }
}
