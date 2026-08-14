namespace GourmetSpot.Services.Contracts
{
    public interface IStoreManager<T>
    {
        string LoadMessage { get; }

        List<T> Load();
        bool Save(List<T> items);
    }
}
