using Arragro.Common.ServiceBase;
using Arragro.ObjectLogging;
using Arragro.ObjectLogging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectLogging.Services
{
    public class ObjectHistoryService : Service<IObjectHistoryRepository, ObjectHistory, Guid>
    {
        public ObjectHistoryService(IObjectHistoryRepository objectHistoryRepository)
            : base(objectHistoryRepository)
        {
        }

        public IEnumerable<ObjectHistory> GetObjectHistories(Type type, string id)
        {
            var key = type.Name + ":" + id.ToString();
            return Repository.All()
                        .Where(o => o.Key == key)
                        .OrderBy(o => o.DateChanged)
                        .ToList();
        }

        public override ObjectHistory InsertOrUpdate(ObjectHistory objectHistory)
        {
            var add = objectHistory.ObjectHistoryId == default(Guid) ? true : false;
            objectHistory.SetObjectObjectHistoryId();
            return Repository.InsertOrUpdate(objectHistory, add);
        }

        protected override void ValidateModelRules(ObjectHistory objectHistory)
        {
            if (RulesException.Errors.Any()) throw RulesException;
        }
    }
}
