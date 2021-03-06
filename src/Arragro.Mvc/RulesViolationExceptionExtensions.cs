﻿using Arragro.Common.BusinessRules;
using Arragro.Mvc.Helpers;
using System.Web.Mvc;

namespace Arragro.Mvc
{
    public static class RulesViolationExceptionExtensions
    {
        public static void CopyTo(this RulesException ex,
                                  Controller controller)
        {
            CopyTo(ex, controller, controller.ModelState, null);
        }

        public static void CopyTo(this RulesException ex,
                                  ModelStateDictionary modelState)
        {
            CopyTo(ex, null, modelState, null);
        }

        public static void CopyTo(this RulesException ex,
                                  Controller controller,
                                  ModelStateDictionary modelState,
                                  string prefix)
        {
            prefix = string.IsNullOrEmpty(prefix) ? "" : prefix + ".";
            foreach (var propertyError in ex.Errors)
            {
                var errorPrefix = string.IsNullOrEmpty(propertyError.Prefix) ? prefix : propertyError.Prefix + ".";
                var key = ExpressionHelper.GetExpressionText(propertyError.Property);
                modelState.AddModelError(errorPrefix + key, propertyError.Message);
            }
            if (controller != null)
                controller.Flash(modelState, FlashEnum.Error);
        }

        public static void CopyTo<TModel>(
            this RulesException<TModel> ex, Controller controller, FlashEnum type = FlashEnum.Error) where TModel : class
        {
            if (ex.Errors.Count > 0)
            {
                controller.Flash(ex.Errors, type);
            }
        }
    }
}
