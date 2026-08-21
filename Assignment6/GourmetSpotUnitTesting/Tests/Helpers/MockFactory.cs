using Moq;
using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Tests.Helpers
{
    public static class MockFactory
    {
        public static Mock<IStoreManager<Order>> CreateOrderStoreMock(
            List<Order>? orders = null,
            bool setupSave = false)
        {
            Mock<IStoreManager<Order>> orderStoreMock =
                new Mock<IStoreManager<Order>>();

            orderStoreMock
                .Setup(store => store.Load())
                .Returns(orders ?? new List<Order>());

            if (setupSave)
            {
                orderStoreMock
                    .Setup(store => store.Save(It.IsAny<List<Order>>()))
                    .Returns(true);
            }

            return orderStoreMock;
        }

        public static Mock<IInventoryManager> CreateInventoryManagerMock()
        {
            return new Mock<IInventoryManager>();
        }
    }
}