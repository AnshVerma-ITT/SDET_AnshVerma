using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Tests.Fakes
{
    internal class FakeOrderStoreManager : IStoreManager<Order>
    {
        private readonly List<Order> storedOrders;
        public string LoadMessage { get; private set; } = string.Empty;
        public bool SaveWasCalled { get; private set; }

        public FakeOrderStoreManager()
        {
            storedOrders = new List<Order>();
        }

        public FakeOrderStoreManager(List<Order> orders)
        {
            storedOrders = new List<Order>(orders);
        }

        public List<Order> Load()
        {
            return new List<Order>(storedOrders);
        }

        public bool Save(List<Order> items)
        {
            SaveWasCalled = true;
            storedOrders.Clear();
            storedOrders.AddRange(items);
            return true;
        }
    }
}
