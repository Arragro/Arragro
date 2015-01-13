using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Arragro.Common.Helpers
{
    public static class ObjectHelpers
    {
        public static PropertyInfo GetKeyProperty<TModel, TKeyType>()
        {
            var type = typeof(TModel);
            var name = type.Name;
            var properties = type.GetProperties();
            var key = properties.SingleOrDefault(x => x.IsDefined(typeof(KeyAttribute), true));

            if (key == null)
                key = properties.SingleOrDefault(x => x.Name == name + "Id");

            if (key == null)
                key = properties.SingleOrDefault(x => x.Name == "Id");

            if (key == null)
                throw new Exception("Cannot find Key, use Id, {Type}Id, or Key attribute");

            if (key.PropertyType != typeof(TKeyType))
                throw new Exception(string.Format("Key is not the same defined on the class {0}", typeof(TKeyType).Name));

            return key;
        }


        public static TKeyType GetKeyPropertyValue<TModel, TKeyType>(TModel model)
        {
            return (TKeyType)GetKeyProperty<TModel, TKeyType>().GetValue(model);
        }
    }
}
