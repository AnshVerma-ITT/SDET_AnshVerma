using System.Reflection;
using System.Runtime.CompilerServices;

namespace GourmetSpot.Tests.Helpers
{
    public abstract class ManagerTestBase<TManager> where TManager : class
    {
        protected TManager Manager { get; private set; } = null!;

        [SetUp]
        public void SetUpManager()
        {
            Manager = CreateManager();
        }

        protected virtual TManager CreateManager() => CreateWithoutConstructor<TManager>();

        protected void SetField<TValue>(string fieldName, TValue value)
        {
            typeof(TManager)
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                .SetValue(Manager, value);
        }

        private static T CreateWithoutConstructor<T>() where T : class =>
            (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }
}
