namespace NHibernate.PersistenceTesting.Utils
{
    public class PropertyChain : Accessor
    {
        private readonly Member[] _chain;
        private readonly SingleMember innerMember;

        public PropertyChain(Member[] members)
        {
            _chain = new Member[members.Length - 1];
            for (int i = 0; i < _chain.Length; i++)
            {
                _chain[i] = members[i];
            }

            innerMember = new SingleMember(members[members.Length - 1]);
        }

        #region Accessor Members

        public void SetValue(object target, object propertyValue)
        {
            target = findInnerMostTarget(target);
            if (target == null)
            {
                return;
            }

            innerMember.SetValue(target, propertyValue);
        }

        public object GetValue(object target)
        {
            target = findInnerMostTarget(target);

            if (target == null)
            {
                return null;
            }

            return innerMember.GetValue(target);
        }

        public System.Type PropertyType
        {
            get { return innerMember.PropertyType; }
        }

        public string Name
        {
            get
            {
                string returnValue = string.Empty;
                foreach (var info in _chain)
                {
                    returnValue += info.Name + ".";
                }

                returnValue += innerMember.Name;

                return returnValue;
            }
        }

        #endregion

        private object findInnerMostTarget(object target)
        {
            foreach (var info in _chain)
            {
                target = info.GetValue(target);
                if (target == null)
                {
                    return null;
                }
            }

            return target;
        }
    }
}
