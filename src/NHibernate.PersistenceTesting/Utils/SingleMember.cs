namespace NHibernate.PersistenceTesting.Utils
{
    public class SingleMember : Accessor
    {
        private readonly Member member;

        public SingleMember(Member member)
        {
            this.member = member;
        }

        #region Accessor Members

        public string FieldName
        {
            get { return member.Name; }
        }

        public System.Type PropertyType
        {
            get { return member.PropertyType; }
        }

        public Member InnerMember
        {
            get { return member; }
        }

        public string Name
        {
            get { return member.Name; }
        }

        public void SetValue(object target, object propertyValue)
        {
            member.SetValue(target, propertyValue);
        }

        public object GetValue(object target)
        {
            return member.GetValue(target);
        }

        #endregion
    }
}
