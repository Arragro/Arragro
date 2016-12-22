using Arragro.TestBase;
using Microsoft.EntityFrameworkCore;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class FooContext : BaseContext
    {
        public FooContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ModelFoo> ModelFoos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
