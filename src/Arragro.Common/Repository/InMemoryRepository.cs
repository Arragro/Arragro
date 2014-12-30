using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Arragro.Common.Repository
{
    public class InMemoryRepository<TModel, TKeyType> : IRepository<TModel, TKeyType> where TModel : class
    {
        private readonly HashSet<TModel> _models;
        private PropertyInfo _keyProperty;

        private PropertyInfo GetKeyProperty()
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

        private TKeyType GetKeyPropertyValue(TModel model)
        {
            return (TKeyType)GetKeyProperty().GetValue(model);
        }

        public InMemoryRepository()
        {
            _models = new HashSet<TModel>();
            _keyProperty = GetKeyProperty();
        }

        public void TurnOnOffLazyLoading(bool on = true)
        {
        }

        private MethodCallExpression GetFindWhereClause(IQueryable<TModel> query, TKeyType id)
        {
            // See: http://msdn.microsoft.com/en-us/library/bb882637.aspx
            // Get the Key Property
            var pe = Expression.Parameter(typeof(TModel));
            // Set the left side of the where clause (the property)
            var left = Expression.Property(pe, _keyProperty.Name);
            // Set the right side of the where clause (the value)
            var right = Expression.Constant(id, typeof(TKeyType));
            // Specify the type of where clause
            var equals = Expression.Equal(left, right);
            // Create the where clause as an expression tree
            var whereClause =
                Expression.Call(
                            typeof(Queryable),
                            "Where",
                            new Type[] { query.ElementType },
                            query.Expression,
                            Expression.Lambda<Func<TModel, bool>>(equals, new ParameterExpression[] { pe }));
            return whereClause;
        }

        public TModel Find(TKeyType id)
        {
            // Turn the HashTable of models into a Queryable
            var query = _models.AsQueryable<TModel>();
            var whereClause = GetFindWhereClause(query, id);
            return query.Provider.CreateQuery<TModel>(whereClause).SingleOrDefault();
        }

        public TModel Delete(TKeyType id)
        {
            var model = Find(id);
            if (model != null) _models.Remove(model);
            return model;
        }

        public IQueryable<TModel> All()
        {
            return _models.AsQueryable();
        }

        private TKeyType GetMaxKeyValueInModels()
        {
            // See: http://msdn.microsoft.com/en-us/library/bb882637.aspx
            // Get the Key Property
            var pe = Expression.Parameter(typeof(TModel));
            var property = Expression.Property(pe, _keyProperty.Name);

            return _models.AsQueryable()
                        .Select(Expression.Lambda<Func<TModel, TKeyType>>(property, pe))
                        .DefaultIfEmpty(default(TKeyType))
                        .Max();
        }

        private TKeyType GetNextValue()
        {
            var type = typeof(TKeyType);
            if (type == typeof(Guid))
                return (TKeyType)(Guid.NewGuid() as object);
            if (type == typeof(int))
                return (TKeyType)(((int)(GetMaxKeyValueInModels() as object) + 1) as object);

            throw new Exception(string.Format("The {0} is not catered for!", type.Name));
        }

        public TModel InsertOrUpdate(TModel model, bool add)
        {
            if (add)
            {
                model.GetType().GetProperty(_keyProperty.Name).SetValue(model, GetNextValue());
                _models.Add(model);
            }
            else
            {
                _models.Remove(Find(GetKeyPropertyValue(model)));
                _models.Add(model);
            }
            return model;
        }

        public int SaveChanges()
        {
            return 0;
        }
    }
}