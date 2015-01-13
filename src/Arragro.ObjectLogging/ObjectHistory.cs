using Arragro.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectLogging
{
    public class ObjectHistory
    {
        public Guid ObjectHistoryId { get; private set; }
        public string Key { get; private set; }
        public string Data { get; private set; }
        public string UserName { get; private set; }
        public DateTime DateChanged { get; private set; }

        private IEnumerable<ComparisonResult> _comparisonResults = null;
        public IEnumerable<ComparisonResult> ComparisonResults
        {
            get
            {
                if (_comparisonResults == null)
                {
                    _comparisonResults = JsonConvert.DeserializeObject<IEnumerable<ComparisonResult>>(Data);
                }
                return _comparisonResults;
            }
        }

        protected ObjectHistory() { }

        public ObjectHistory(Type type, string id, IEnumerable<ComparisonResult> comparisonResults, string userName)
        {
            ObjectHistoryId = new Guid();
            Key = type.Name + ":" + id;
            Data = JsonConvert.SerializeObject(comparisonResults);
            UserName = userName;
            DateChanged = DateTime.Now;
        }

        public void SetObjectObjectHistoryId()
        {
            if (ObjectHistoryId == default(Guid))
                ObjectHistoryId = Guid.NewGuid();
        }
    }

    public static class ObjectHistoryExtensions
    {
        public static ObjectHistory BuildObjectHistory<TModel, TKeyType>(TModel originalValue, TModel newValue, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");

            var object1KeyValue = ObjectHelpers.GetKeyPropertyValue<TModel, TKeyType>(originalValue);
            var object2KeyValue = ObjectHelpers.GetKeyPropertyValue<TModel, TKeyType>(newValue);

            if (!object1KeyValue.Equals(object2KeyValue))
                throw new ArgumentException("You are trying to compare the same object with a different key, this is not allowed!");

            var comparisonResults = CompareEngine.Compare(originalValue, newValue);

            return new ObjectHistory(originalValue.GetType(), object1KeyValue.ToString(), comparisonResults, userName);
        }

        public static ObjectHistory BuildObjectHistory<TModel, TKeyType>(TModel object1, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");

            var object1KeyValue = ObjectHelpers.GetKeyPropertyValue<TModel, TKeyType>(object1);

            var comparisonResults = CompareEngine.Compare(object1);

            return new ObjectHistory(object1.GetType(), object1KeyValue.ToString(), comparisonResults, userName);
        }
    }
}
