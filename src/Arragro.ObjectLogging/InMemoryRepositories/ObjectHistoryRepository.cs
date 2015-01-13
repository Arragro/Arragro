using Arragro.Common.Repository;
using Arragro.ObjectLogging;
using Arragro.ObjectLogging.Interfaces;
using System;

namespace Bayleys.People.And.Places.DataLayer.InMemoryRepositories
{
    public class ObjectHistoryRepository : InMemoryRepository<ObjectHistory, Guid>, IObjectHistoryRepository
    {
    }
}