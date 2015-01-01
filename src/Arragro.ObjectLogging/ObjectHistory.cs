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
        public int ObjectHistoryId { get; private set; }
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
            Key = type.Name + ":" + id;
            Data = JsonConvert.SerializeObject(comparisonResults);
            UserName = userName;
            DateChanged = DateTime.Now;
        }
    }
}
