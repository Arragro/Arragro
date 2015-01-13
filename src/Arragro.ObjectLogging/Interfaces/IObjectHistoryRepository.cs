using Arragro.Common.Repository;
using Arragro.ObjectLogging;
using System;

namespace Arragro.ObjectLogging.Interfaces
{
    public interface IObjectHistoryRepository : IRepository<ObjectHistory, Guid>
    {
    }
}
