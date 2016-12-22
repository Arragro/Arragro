using Arragro.TestBase;
using ArragroCMS.Data.EFCore;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class ModelFooRepository : DbContextRepositoryBase<ModelFoo>
    {
        public ModelFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
