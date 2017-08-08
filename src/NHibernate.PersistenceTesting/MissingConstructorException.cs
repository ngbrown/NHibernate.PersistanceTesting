using System;
using System.Runtime.Serialization;

namespace NHibernate.PersistenceTesting
{
    [Serializable]
    public class MissingConstructorException : Exception
    {
        public MissingConstructorException(System.Type type)
            : base("'" + type.AssemblyQualifiedName + "' is missing a parameterless constructor.")
        {}

        protected MissingConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {}
    }
}
