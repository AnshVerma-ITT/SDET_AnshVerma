using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Assignment1_OOP_DailyActivities.Model;

namespace Assignment1_OOP_DailyActivities.Utilities
{
    public class JsonTaskFile
    {
        private string filePath;

        public JsonTaskFile()
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Task.json");
        }

        public List<DailyTask> LoadTasks()
        {
            try
            {
                CreateFileIfMissing();
                string json = File.ReadAllText(filePath);
                if (json.Trim() == "")
                {
                    return new List<DailyTask>();
                }
                JsonSerializerOptions options = GetJsonOptions();
                List<DailyTask> dailyTasks = JsonSerializer.Deserialize<List<DailyTask>>(json, options);
                if (dailyTasks == null)
                {
                    return new List<DailyTask>();
                }
                return dailyTasks;
            }
            catch (JsonException)
            {
                throw new Exception("Task data file has invalid JSON.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveTasks(List<DailyTask> dailyTasks)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(filePath);
                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }
                JsonSerializerOptions options = GetJsonOptions();
                string json = JsonSerializer.Serialize(dailyTasks, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateFileIfMissing()
        {
            try
            {
                string folderPath = Path.GetDirectoryName(filePath);
                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }
                if (File.Exists(filePath) == false)
                {
                    File.WriteAllText(filePath, "[]");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private JsonSerializerOptions GetJsonOptions()
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                options.IncludeFields = true;
                return options;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
