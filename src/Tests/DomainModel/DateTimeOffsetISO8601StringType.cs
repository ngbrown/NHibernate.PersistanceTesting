using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Tests.DomainModel
{
    /// <summary>
    /// SQLite doesn't support DateTimeOffset, so simulate with a string.
    /// </summary>
    public class DateTimeOffsetISO8601StringType : DateTimeOffsetType
    {
        private const string DateTimeOffsetStringFormat = "yyyy-MM-ddTHH\\:mm\\:sszzz";

        public override Type PrimitiveClass
        {
            get { return typeof(string); }
        }

        public override SqlType SqlType
        {
            get { return SqlTypeFactory.GetAnsiString(35); }
        }

        public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
        {
            var dateTimeOffset = (DateTimeOffset)value;
            // purpously shortened to simulate saving to a server that supports DateTimeOffset, but not to the full resolution.
            ((IDataParameter)st.Parameters[index]).Value = dateTimeOffset.ToString(DateTimeOffsetStringFormat, CultureInfo.InvariantCulture);
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            try
            {
                var r = DateTimeOffset.ParseExact((string)rs[index], DateTimeOffsetStringFormat, CultureInfo.InvariantCulture);
                return new DateTimeOffset(r.Ticks, r.Offset);
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
            }
        }
    }
}