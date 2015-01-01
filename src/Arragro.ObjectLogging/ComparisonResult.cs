using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectLogging
{
    public class ComparisonResult
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }
        public object OriginalValue { get; private set; }
        public object NewValue { get; private set; }

        public ComparisonResult(
            Type type,
            string name,
            object originalValue,
            object newValue)
        {
            Type = type;
            Name = name;
            OriginalValue = originalValue;
            NewValue = newValue;
        }
    }
}
