using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.PersistenceTesting;
using NUnit.Framework;
using Tests.DomainModel;

namespace Tests.Testing
{
    [TestFixture]
    public class when_building_a_session_source : with_fluent_configuration
    {
        private ISessionSource _sessionSource;

        public override void establish_context()
        {
            _sessionSource = build_session_source();
        }

        [Test]
        public void should_be_able_to_get_a_new_session()
        {
            _sessionSource.GetSession();
        }

        [Test]
        public void should_be_able_to_generate_the_schema()
        {
            _sessionSource.BuildSchema();
        }
    }

    [TestFixture]
    public class when_using_a_session_source_and_schema : with_fluent_configuration
    {
        private ISessionSource _sessionSource;

        public override void establish_context()
        {
            _sessionSource = build_session_source();
            _sessionSource.BuildSchema();
        }

        [Test]
        public void should_be_able_to_use_the_fluent_mappings()
        {
            new PersistenceSpecification<Record>(_sessionSource.GetSession())
                .CheckProperty(x => x.Name, "Luke Skywalker")
                .CheckProperty(x => x.Age, 18)
                .CheckProperty(x => x.Location, "Tatooine")
                .VerifyTheMappings();
        }
    }

    public class with_fluent_configuration : Specification
    {
        public ISessionSource build_session_source()
        {
            var configuration = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.Driver<SQLite20Driver>();
                    db.Dialect<SQLiteDialect>();
                    db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
                    db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                });

            var mapper = new ModelMapper();
            mapper.AddMapping<RecordMap>();
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            return new SingleConnectionSessionSourceForSQLiteInMemoryTesting(configuration);
        }
    }
}