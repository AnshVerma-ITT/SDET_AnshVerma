namespace DailyActivityTracker.Model
{
    public class DailyTask : TaskItem
    {
        public DailyTask()
            : base()
        {
        }

        public DailyTask(int id, string title, string category, string time)
            : base(id, title, category, time)
        {
        }
    }
}
