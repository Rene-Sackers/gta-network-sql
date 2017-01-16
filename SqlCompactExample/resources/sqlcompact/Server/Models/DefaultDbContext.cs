using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServerCompact;

namespace SqlCompactExample.resources.sqlcompact.Server.Models
{

    [DbConfigurationType(typeof(DatabaseConfiguration))]
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    public class ContextFactory : IDbContextFactory<DefaultDbContext>
    {
        public static string DatabaseFilePath;

        public static DefaultDbContext Instance
        {
            get
            {
                if (DatabaseFilePath == null) throw new InvalidOperationException("Tried to use database context before setting file path.\nMake sure you set the DB file path on the ContextFactory in the onResourceStart event, before using it!");

                return new DefaultDbContext("Data Source=" + DatabaseFilePath);
            }
        }

        public ContextFactory()
        {
        }

        public DefaultDbContext Create()
        {
            return Instance;
        }
    }

    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<DefaultDbContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }

    internal sealed class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration()
        {
            SetProviderServices(SqlCeProviderServices.ProviderInvariantName, SqlCeProviderServices.Instance);
            SetDefaultConnectionFactory(new SqlCeConnectionFactory(SqlCeProviderServices.ProviderInvariantName));
        }
    }
}
