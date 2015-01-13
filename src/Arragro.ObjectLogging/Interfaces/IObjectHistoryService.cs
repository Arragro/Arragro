using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectLogging.Interfaces
{
    public interface IObjectHistoryService
    {
        ObjectHistory AddHistory<TModel, TKeyType>(TModel newObject, string userName);
        ObjectHistory AddHistory<TModel, TKeyType>(TModel originalObject, TModel newObject, string userName);
        IEnumerable<ObjectHistory> GetObjectHistories(Type type, string id);
    }
}
