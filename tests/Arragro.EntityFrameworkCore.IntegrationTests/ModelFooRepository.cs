using Arragro.TestBase;
using Arragro.EntityFrameworkCore;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class ModelFooRepository : DbContextRepositoryBase<ModelFoo>
    {
        public ModelFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
