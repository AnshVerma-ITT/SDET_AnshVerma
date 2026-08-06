namespace DailyActivityTracker.Model
{
    public abstract class TaskItem
    {
        public int Id { get; private set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Time { get; set; }
        public bool IsCompleted { get; set; }

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
    }
}
