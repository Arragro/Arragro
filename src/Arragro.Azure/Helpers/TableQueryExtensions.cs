using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Azure.Helpers
{
    public static class TableQueryExtensions
    {
        public static TableQuery<TElement> AndWhere<TElement>(this TableQuery<TElement> @this, string filter) where TElement: class, ITableEntity, new()
        {
            if (string.IsNullOrEmpty(@this.FilterString))
                @this.FilterString = filter;
            else
                @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.And, filter);
            return @this;
        }

        public static TableQuery<TElement> OrWhere<TElement>(this TableQuery<TElement> @this, string filter) where TElement : class, ITableEntity, new()
        {
            if (string.IsNullOrEmpty(@this.FilterString))
                @this.FilterString = filter;
            else
                @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.Or, filter);
            return @this;
        }
    }
}
