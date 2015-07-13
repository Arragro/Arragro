using Arragro.TestBase;

namespace Arragro.EF6.IntegrationTests
{
    public class ModelFooRepository : DbContextRepositoryBase<ModelFoo, int>
    {
        public ModelFooRepository(FooContext fooContext) : base(fooContext) { }
    }
}
