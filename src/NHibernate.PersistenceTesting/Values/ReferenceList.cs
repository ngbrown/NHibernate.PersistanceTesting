using System.Collections.Generic;
using NHibernate.PersistenceTesting.Utils;

namespace NHibernate.PersistenceTesting.Values
{
    public class ReferenceList<T, TListElement> : List<T, TListElement>
        where T : class
        where TListElement : class
    {
        public ReferenceList(Accessor property, IEnumerable<TListElement> value)
            : base(property, value)
        {}

        public override void HasRegistered(PersistenceSpecification<T> specification)
        {
            if (Expected == null) return;
            foreach (TListElement item in Expected)
            {
                specification.TransactionalSave(item);
            }
        }
    }
}
