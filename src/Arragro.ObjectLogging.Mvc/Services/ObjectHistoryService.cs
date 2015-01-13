using Arragro.Common.ServiceBase;
using Arragro.ObjectLogging;
using Arragro.ObjectLogging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.ObjectLogging.Mvc.Services
{
    public class ObjectHistoryService : Service<IObjectHistoryRepository, ObjectHistory, Guid>, IObjectHistoryService
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

        public ObjectHistory AddHistory<TModel, TKeyType>(TModel newObject, string userName)
        {
            var objectHistory = InsertOrUpdate(ObjectHistoryExtensions.BuildObjectHistory<TModel, TKeyType>(newObject, userName));
            SaveChanges();
            return objectHistory;
        }

        public ObjectHistory AddHistory<TModel, TKeyType>(TModel originalObject, TModel newObject, string userName)
        {
            var objectHistory = InsertOrUpdate(ObjectHistoryExtensions.BuildObjectHistory<TModel, TKeyType>(originalObject, newObject, userName));
            SaveChanges();
            return objectHistory;
        }
    }
}
