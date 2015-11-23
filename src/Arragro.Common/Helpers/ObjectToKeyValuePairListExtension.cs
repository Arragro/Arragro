using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Arragro.Common.Helpers
{
    public static class ObjectToKeyValuePairListExtension
    {

        public static List<KeyValuePair<string, object>> ToKeyValuePairList(this object source)
        {
            return source.ToKeyValuePairList<object>();
        }

        public static List<KeyValuePair<string, object>> ToKeyValuePairList<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var keyValuePairs = new List<KeyValuePair<string, object>>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToKeyValuePair<T>(property, source, keyValuePairs);

            return keyValuePairs;
        }

        private static void AddPropertyToKeyValuePair<T>(PropertyDescriptor property, object source, List<KeyValuePair<string, object>> keyValuePairs)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value) && value != null)
            {
                if (value is string)
                {
                    keyValuePairs.Add(new KeyValuePair<string, object>(property.Name, (T)value));
                }
                else if (value is IEnumerable)
                {
                    IEnumerable enumerable = (value as IEnumerable);
                    int count = 0;
                    foreach (var item in enumerable)
                    {
                        keyValuePairs.Add(new KeyValuePair<string, object>(property.Name + "[" + count.ToString() + "]", (T)item));
                        count++;
                    }
                }
                else
                    keyValuePairs.Add(new KeyValuePair<string, object>(property.Name, (T)value));
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}
