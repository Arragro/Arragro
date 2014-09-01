using System;
namespace Arragro.Common.Tests.Unity.UseCases
{
    interface IModelFooService
    {
        void EnsureValidModel(Arragro.Common.Tests.ModelsAndHelpers.ModelFoo model, params object[] relatedModels);
        Arragro.Common.Tests.ModelsAndHelpers.ModelFoo InsertOrUpdate(Arragro.Common.Tests.ModelsAndHelpers.ModelFoo model);
    }
}
