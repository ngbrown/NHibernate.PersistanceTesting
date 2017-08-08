using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;

namespace Tests
{
    public interface ISessionSource
    {
        ISession GetSession();
        void BuildSchema();

        Configuration Configuration { get; }
        ISessionFactory SessionFactory { get; }
    }

    public class SingleConnectionSessionSourceForSQLiteInMemoryTesting : ISessionSource
    {
        public ISessionFactory SessionFactory { get; private set; }
        public Configuration Configuration { get; private set; }
        private ISession session;

        public SingleConnectionSessionSourceForSQLiteInMemoryTesting(Configuration config, HbmMapping model)
        {
            config.AddMapping(model);

            Configuration = config;
            SessionFactory = config.BuildSessionFactory();
        }

        public SingleConnectionSessionSourceForSQLiteInMemoryTesting(Configuration config)
        {
            Configuration = config;
            SessionFactory = config.BuildSessionFactory();
        }


        public ISession GetSession()
        {
            if (session == null)
            {
                session = SessionFactory.OpenSession();
            }

            session.Clear();
            return session;
        }

        public void BuildSchema()
        {
            BuildSchema(GetSession());
        }

        public void BuildSchema(bool script)
        {
            BuildSchema(GetSession(), script);
        }


        public void BuildSchema(ISession session)
        {
            BuildSchema(session, false);
        }

        public void BuildSchema(ISession session, bool script)
        {
            new SchemaExport(Configuration)
                .Execute(script, true, false, session.Connection, null);
        }
    }
}