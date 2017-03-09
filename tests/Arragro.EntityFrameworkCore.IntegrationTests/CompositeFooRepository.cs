using Arragro.TestBase;
using ArragroCMS.Data.EFCore;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class CompositeFooRepository : DbContextRepositoryBase<CompositeFoo>
    {
        public CompositeFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
