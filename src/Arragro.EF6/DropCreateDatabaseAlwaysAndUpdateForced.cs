using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Arragro.EF6
{
    public class DropCreateDatabaseAlwaysAndUpdateForced<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly DbMigrationsConfiguration _dbMigrationsConfiguration;

        public DropCreateDatabaseAlwaysAndUpdateForced(DbMigrationsConfiguration dbMigrationsConfiguration)
        {
            if (dbMigrationsConfiguration == null)
                throw new ArgumentNullException("dbMigrationsConfiguration");
            _dbMigrationsConfiguration = dbMigrationsConfiguration;
        }

        public void SetDatabaseToSingleUserAndDrop(TContext context)
        {
            var databaseName = context.Database.Connection.Database;
            const string sqlCommandText = @"
ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";

            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, string.Format(sqlCommandText, databaseName));
            context.Database.Delete();
        }

        public void InitializeDatabase(TContext context)
        {
            // Create our database if it doesn't already exist.
            if (context.Database.Exists())
                SetDatabaseToSingleUserAndDrop(context);
            //context.Database.Create();

            // Do you want to migrate to latest in your initializer? Add code here!
            DatabaseMigrator.Migrate(_dbMigrationsConfiguration);
        }
    }
}
