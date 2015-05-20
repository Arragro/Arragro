using System.Data.Entity.Migrations;

namespace Arragro.EF6
{
    public static class DatabaseMigrator
    {
        public static void Migrate(DbMigrationsConfiguration dbMigrationConfiguration)
        {
            var migrator = new DbMigrator(dbMigrationConfiguration);
            migrator.Configuration.AutomaticMigrationsEnabled = true;
            migrator.Update();
        }
    }
}
