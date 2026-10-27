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

#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
        protected MissingConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {}
    }
}
