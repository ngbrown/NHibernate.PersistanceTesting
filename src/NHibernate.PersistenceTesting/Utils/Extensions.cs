using NHibernate.Util;

namespace NHibernate.PersistenceTesting.Utils
{
    public static class Extensions
    {
        public static T InstantiateUsingParameterlessConstructor<T>(this System.Type type)
        {
            return (T)type.InstantiateUsingParameterlessConstructor();
        }

        public static object InstantiateUsingParameterlessConstructor(this System.Type type)
        {
            var constructor = ReflectHelper.GetDefaultConstructor(type);

            if (constructor == null)
                throw new MissingConstructorException(type);

            return constructor.Invoke(null);
        }
    }
}
