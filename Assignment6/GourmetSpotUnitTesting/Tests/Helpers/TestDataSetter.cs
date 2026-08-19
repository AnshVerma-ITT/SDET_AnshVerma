using System.Reflection;
using System.Runtime.CompilerServices;

namespace GourmetSpot.Tests.Helpers
{
    internal static class TestDataSetter
    {
        public static T CreateWithoutConstructor<T>() where T : class
        {
            return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        }

        public static void SetField<TTarget, TValue>(
            TTarget target,
            string fieldName,
            TValue value) where TTarget : class
        {
            FieldInfo? fieldInfo = typeof(TTarget).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            fieldInfo!.SetValue(target, value);
        }
    }
}
