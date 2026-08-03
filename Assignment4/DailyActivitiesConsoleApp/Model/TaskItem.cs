namespace Assignment1_OOP_DailyActivities.Model
{
    public abstract class TaskItem
    {
        public int Id;
        public string Title;
        public string Category;
        public string Time;
        public bool IsCompleted;

        protected TaskItem()
        {
            Id = 0;
            Title = string.Empty;
            Category = string.Empty;
            Time = string.Empty;
            IsCompleted = false;
        }

        protected TaskItem(int id, string title, string category, string time)
        {
            Id = id;
            Title = title;
            Category = category;
            Time = time;
            IsCompleted = false;
        }

        public virtual string GetStatus()
        {
            return IsCompleted ? "Completed" : "Pending";
        }
    }
}
