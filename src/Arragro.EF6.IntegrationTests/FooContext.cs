using Arragro.TestBase;
using System.Data.Entity;

namespace Arragro.EF6.IntegrationTests
{
    public class FooContext : BaseContext
    {
        public IDbSet<ModelFoo> ModelFoos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
