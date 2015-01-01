using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arragro.ObjectLogging
{
    public static class CompareEngine
    {
        private struct TempCompareResult
        {
            public Type Type { get; set; }
            public string Name { get; set; }
            public object OriginalValue { get; set; }
            public object NewValue { get; set; }
        }

        private static bool CanCompareDirectly(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        private static bool AreValuesEqual(TempCompareResult tempCompareResult)
        {
            IComparable selfValueComparer;

            var valueA = tempCompareResult.OriginalValue;
            var valueB = tempCompareResult.NewValue;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                return false;
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                return false;
            else if (!object.Equals(valueA, valueB))
                return false; 
            else
                return true;
        }

        private static IEnumerable<TempCompareResult> GetProperties(object object1, object object2)
        {
            return (
                from p1 in object1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                join p2 in object2.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance) on p1.Name equals p2.Name
                where p1.CanRead
                   && p1.GetSetMethod() != null
                   && p2.GetGetMethod() != null
                select new TempCompareResult
                {
                    Name = p1.Name,
                    Type = p2.PropertyType,
                    OriginalValue = p1.GetValue(object1, null),
                    NewValue = p2.GetValue(object2, null)
                });
        }

        private static IEnumerable<TempCompareResult> GetFields(object object1, object object2)
        {
            return (
                from v1 in object1.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                join v2 in object2.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance) on v1.Name equals v2.Name
                select new TempCompareResult
                { 
                    Name = v1.Name, 
                    Type = v1.FieldType, 
                    OriginalValue = v1.GetValue(object1), 
                    NewValue = v2.GetValue(object2) 
                });
        }

        public static IEnumerable<ComparisonResult> Compare(object object1, object object2)
        {
            var properties = GetProperties(object1, object2);
            var fields = GetFields(object1, object2);

            var results = (
                from p in properties.Union(fields)
                where !AreValuesEqual(p)
                select new ComparisonResult(
                    p.Type, p.Name, p.OriginalValue, p.NewValue));

            return results;
        }
    }
}
