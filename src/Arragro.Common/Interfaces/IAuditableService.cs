using Arragro.Common.BusinessRules;

namespace Arragro.Common.Interfaces
{
    public interface IAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
    {
        TModel Find(params object[] ids);
        TModel InsertOrUpdate(TModel model, TUserIdType userId);
        TModel ValidateAndInsertOrUpdate(TModel model, TUserIdType userId);
        void ValidateModel(TModel model);
        int SaveChanges();
    }
}