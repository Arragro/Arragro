using Arragro.EF6;
using Arragro.ObjectLogging;
using Arragro.ObjectLogging.Interfaces;
using System;

namespace Arragro.ObjectLogging.Mvc.EF6Repositories
{
    public class ObjectHistoryRepository<TContext> : DbContextRepositoryBase<ObjectHistory, Guid>, IObjectHistoryRepository where TContext : BaseContext
    {
        public ObjectHistoryRepository(TContext context) : base(context) { }
    }
}
