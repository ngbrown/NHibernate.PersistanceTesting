namespace NHibernate.PersistenceTesting.Utils
{
    public interface Accessor
    {
        System.Type PropertyType { get; }
        void SetValue(object target, object propertyValue);
        object GetValue(object target);

        string Name { get; }
    }
}
