using Arragro.TestBase;
using Arragro.EntityFrameworkCore;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class CompositeFooRepository : DbContextRepositoryBase<CompositeFoo>
    {
        public CompositeFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
