using System;
using System.Collections.Generic;
using Assignment1_OOP_DailyActivities.Model;
using Assignment1_OOP_DailyActivities.Utilities;

namespace Assignment1_OOP_DailyActivities.Services
{
    public class TaskService
    {
        private List<DailyTask> dailyTasks;
        private JsonTaskFile jsonTaskFile;
        private int nextId;

        public TaskService()
        {
            jsonTaskFile = new JsonTaskFile();
            dailyTasks = jsonTaskFile.LoadTasks();
            nextId = GetNextId();
        }

        public void AddTask(string title, string category, string time)
        {
            try
            {
                Validator.ValidateText(title, "Task title");
                Validator.ValidateText(category, "Category");
                Validator.ValidateText(time, "Time");

                DailyTask dailyTask = new DailyTask(nextId, title, category, time);
                dailyTasks.Add(dailyTask);
                nextId++;
                jsonTaskFile.SaveTasks(dailyTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DailyTask> GetAllTasks()
        {
            try
            {
                return dailyTasks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DailyTask GetTaskById(int id)
        {
            try
            {
                Validator.ValidateId(id);

                for (int index = 0; index < dailyTasks.Count; index++)
                {
                    if (dailyTasks[index].Id == id)
                    {
                        return dailyTasks[index];
                    }
                }

                throw new Exception("Task was not found.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void MarkTaskCompleted(int id)
        {
            try
            {
                DailyTask dailyTask = GetTaskById(id);
                dailyTask.IsCompleted = true;
                jsonTaskFile.SaveTasks(dailyTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteTask(int id)
        {
            try
            {
                DailyTask dailyTask = GetTaskById(id);
                dailyTasks.Remove(dailyTask);
                jsonTaskFile.SaveTasks(dailyTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DailyTask> GetPendingTasks()
        {
            try
            {
                List<DailyTask> pendingTasks = new List<DailyTask>();

                for (int index = 0; index < dailyTasks.Count; index++)
                {
                    if (dailyTasks[index].IsCompleted == false)
                    {
                        pendingTasks.Add(dailyTasks[index]);
                    }
                }

                return pendingTasks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DailyTask> GetCompletedTasks()
        {
            try
            {
                List<DailyTask> completedTasks = new List<DailyTask>();

                for (int index = 0; index < dailyTasks.Count; index++)
                {
                    if (dailyTasks[index].IsCompleted == true)
                    {
                        completedTasks.Add(dailyTasks[index]);
                    }
                }

                return completedTasks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetNextId()
        {
            try
            {
                int highestId = 0;

                for (int index = 0; index < dailyTasks.Count; index++)
                {
                    if (dailyTasks[index].Id > highestId)
                    {
                        highestId = dailyTasks[index].Id;
                    }
                }

                return highestId + 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
