using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.PersistenceTesting;
using NHibernate.Type;
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
            mapper.AddMapping<DateTimeRecordMap>();
            mapper.AddMapping<DateTimeOffsetRecordMap>();
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
        public void MappingTest2()
        {
            var record = new Record()
            {
                Age = 22,
                Name = "somebody",
                Location = "somewhere",
            };

            new PersistenceSpecification<Record>(source.GetSession())
                .CheckProperty(() => record.Age)
                .CheckProperty(() => record.Name)
                .CheckProperty(() => record.Location)
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

        [Test]
        public void MappingTest_DateProperty()
        {
            new PersistenceSpecification<DateTimeRecord>(source.GetSession())
                .CheckProperty(x => x.DateValue, DateTime.Now, new DateTimeEqualityComparer(TimeSpan.FromSeconds(1)))
                .VerifyTheMappings();
        }

        [Test]
        public void MappingTest_DateTimeOffsetProperty()
        {
            new PersistenceSpecification<DateTimeOffsetRecord>(source.GetSession())
                .CheckProperty(x => x.DateValue, DateTimeOffset.Now, new DateTimeOffsetEqualityComparer(TimeSpan.FromSeconds(1)))
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

    public class DateTimeRecordMap : ClassMapping<DateTimeRecord>
    {
        public DateTimeRecordMap()
        {
            Id(x => x.Id, map => map.Column("id"));
            Property(x => x.DateValue, map =>
            {
                map.NotNullable(true);
                map.Type<LocalDateTimeType>();
            });
        }
    }

    public class DateTimeRecord : Entity
    {
        public virtual DateTime DateValue { get; set; }
    }

    public class DateTimeOffsetRecordMap : ClassMapping<DateTimeOffsetRecord>
    {
        public DateTimeOffsetRecordMap()
        {
            Id(x => x.Id, map => map.Column("id"));
            Property(x => x.DateValue, map =>
            {
                map.NotNullable(true);
                map.Type<DateTimeOffsetISO8601StringType>();
            });
        }
    }

    public class DateTimeOffsetRecord : Entity
    {
        public virtual DateTimeOffset DateValue { get; set; }
    }
}