using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.PersistenceTesting;
using NUnit.Framework;

namespace Tests.DomainModel
{
    [TestFixture]
    public class ConnectedTester
    {
        private ISessionSource source;

        [SetUp]
        public void SetUp()
        {
            var configuration = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.Driver<SQLite20Driver>();
                    db.Dialect<SQLiteDialect>();
                    db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
                    db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                });

            var mapping = CreateTestPersistenceModel();

            source = new SingleConnectionSessionSourceForSQLiteInMemoryTesting(configuration, mapping);
            source.BuildSchema();
        }

        private static HbmMapping CreateTestPersistenceModel()
        {
            var mapper = new ModelMapper();
            mapper.AddMapping<NestedSubClassMap>();
            mapper.AddMapping<ChildRecordMap>();
            mapper.AddMapping<Child2RecordMap>();
            mapper.AddMapping<RecordMap>();
            mapper.AddMapping<BinaryRecordMap>();
            mapper.AddMapping<RecordWithNullablePropertyMap>();
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            mapping.defaultlazy = true;
            return mapping;
        }

        [Test]
        public void MappingTest1()
        {
            new PersistenceSpecification<Record>(source.GetSession())
                .CheckProperty(r => r.Age, 22)
                .CheckProperty(r => r.Name, "somebody")
                .CheckProperty(r => r.Location, "somewhere")
                .VerifyTheMappings();
        }

        [Test]
        public void Mapping_test_with_arrays()
        {
            new PersistenceSpecification<BinaryRecord>(source.GetSession())
                .CheckProperty(r => r.BinaryValue, new byte[] { 1, 2, 3 })
                .VerifyTheMappings();
        }

        [Test]
        public void CanWorkWithNestedSubClasses()
        {
            new PersistenceSpecification<Child2Record>(source.GetSession())
                .CheckProperty(r => r.Name, "Foxy")
                .CheckProperty(r => r.Another, "Lady")
                .CheckProperty(r => r.Third, "Yeah")
                .VerifyTheMappings();
        }

        [Test]
        public void MappingTest2_NullableProperty()
        {
            new PersistenceSpecification<RecordWithNullableProperty>(source.GetSession())
                .CheckProperty(x => x.Age, null)
                .CheckProperty(x => x.Name, "somebody")
                .CheckProperty(x => x.Location, "somewhere")
                .VerifyTheMappings();
        }
    }

    public class NestedSubClassMap : ClassMapping<SuperRecord>
    {
        public NestedSubClassMap()
        {
            Id(x => x.Id);
            Property(x => x.Name);
            Discriminator(x =>
            {
                x.Column("Type");
                //x.Type(NHibernateUtil.String);
            });
        }
    }

    public class ChildRecordMap : SubclassMapping<ChildRecord>
    {
        public ChildRecordMap()
        {
            DiscriminatorValue("ChildRecord");
            Property(x => x.Another);
        }
    }

    public class Child2RecordMap : SubclassMapping<Child2Record>
    {
        public Child2RecordMap()
        {
            DiscriminatorValue("Child2Record");
            Property(x => x.Third);
        }
    }

    public class SuperRecord : Entity
    {
        public virtual string Name { get; set; }
    }

    public class ChildRecord : SuperRecord
    {
        public virtual string Another { get; set; }
    }

    public class Child2Record : ChildRecord
    {
        public virtual string Third { get; set; }
    }

    public class RecordMap : ClassMapping<Record>
    {
        public RecordMap()
        {
            Id(x => x.Id, map => map.Column("id"));
            Property(x => x.Name);
            Property(x => x.Age);
            Property(x => x.Location);
        }
    }

    public class Record : Entity
    {
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual string Location { get; set; }
    }

    public class BinaryRecordMap : ClassMapping<BinaryRecord>
    {
        public BinaryRecordMap()
        {
            Id(x => x.Id, map => map.Column("id"));
            Property(x => x.BinaryValue, map => map.NotNullable(true));
        }
    }

    public class BinaryRecord : Entity
    {
        public virtual byte[] BinaryValue { get; set; }
    }

    public class RecordWithNullablePropertyMap : ClassMapping<RecordWithNullableProperty>
    {
        public RecordWithNullablePropertyMap()
        {
            Id(x => x.Id);
            Property(x => x.Name);
            Property(x => x.Age);
            Property(x => x.Location);
        }
    }

    public class RecordWithNullableProperty : Entity
    {
        public virtual string Name { get; set; }
        public virtual int? Age { get; set; }
        public virtual string Location { get; set; }
    }
}