namespace Assignment1_OOP_DailyActivities.Model
{
    public class DailyTask
    {
        public int Id;
        public string Title;
        public string Category;
        public string Time;
        public bool IsCompleted;

        public DailyTask()
        {
            Id = 0;
            Title = "";
            Category = "";
            Time = "";
            IsCompleted = false;
        }

        public DailyTask(int id, string title, string category, string time)
        {
            Id = id;
            Title = title;
            Category = category;
            Time = time;
            IsCompleted = false;
        }
    }
}
