using System;
using System.Collections;
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

        private static bool ClassesAreEqual(object objectA, object objectB)
        {
            var objectType = objectA.GetType();

            if (objectType.Name.ToLower() == "string")
                return object.Equals(objectA, objectB);

            foreach (PropertyInfo propertyInfo in objectType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance).Where(
                p => p.CanRead))
            {
                if (!AreValuesEqual(
                            propertyInfo.GetValue(objectA, null),
                            propertyInfo.GetValue(objectB, null)))
                    return false;
            }
            return true;
        }

        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (valueA != null && valueA.GetType().IsClass)
                result = ClassesAreEqual(valueA, valueB);
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

        private static bool AreValuesEqual(TempCompareResult tempCompareResult)
        {
            var valueA = tempCompareResult.OriginalValue;
            var valueB = tempCompareResult.NewValue;

            return AreValuesEqual(valueA, valueB);
        }

        private static IEnumerable<TempCompareResult> GetProperties(object object1, object object2)
        {
            return (
                from p1 in object1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                join p2 in object2.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance) on p1.Name equals p2.Name
                where p1.CanRead
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

        private static IEnumerable<TempCompareResult> GetProperties(object object1)
        {
            return (
                from p1 in object1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where p1.CanRead
                   && p1.GetGetMethod() != null
                select new TempCompareResult
                {
                    Name = p1.Name,
                    Type = p1.PropertyType,
                    OriginalValue = null,
                    NewValue = p1.GetValue(object1, null)
                });
        }

        private static IEnumerable<TempCompareResult> GetFields(object object1)
        {
            return (
                from v1 in object1.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                select new TempCompareResult
                {
                    Name = v1.Name,
                    Type = v1.FieldType,
                    OriginalValue = null,
                    NewValue = v1.GetValue(object1)
                });
        }

        private static IEnumerable<ComparisonResult> GetComparisonResults(
            IEnumerable<TempCompareResult> properties,
            IEnumerable<TempCompareResult> fields)
        {
            var results = new List<ComparisonResult>();

            foreach (var p in properties.Union(fields))
                if (!AreValuesEqual(p))
                    results.Add(new ComparisonResult(
                        p.Type, p.Name, p.OriginalValue, p.NewValue));

            return results;
        }

        public static IEnumerable<ComparisonResult> Compare(object object1, object object2)
        {
            var properties = GetProperties(object1, object2);
            var fields = GetFields(object1, object2);

            return GetComparisonResults(properties, fields);
        }

        public static IEnumerable<ComparisonResult> Compare(object object1)
        {
            var properties = GetProperties(object1);
            var fields = GetFields(object1);

            return GetComparisonResults(properties, fields);
        }
    }
}
