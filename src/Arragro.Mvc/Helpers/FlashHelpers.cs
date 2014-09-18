﻿using Arragro.Common.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

/*

Add the following styles:

#flash
{
    padding: 8px 35px 8px 14px;
    margin-bottom: 18px;
    border: 1px solid;
}

.flash-success
{
    color: #468847;
    background-color: #DFF0D8;
    border-color: #D6E9C6;
}

.flash-info
{
    color: #3A87AD;
    background-color: #D9EDF7;
    border-color: #BCE8F1;
}

.flash-warning
{
    color: #C09853;
    background-color: #FCF8E3;
    border-color: #FBEED5;
}

.flash-error
{
    color: #B94A48;
    background-color: #F2DEDE;
    border-color: #EED3D7;
}
 
*/
namespace Arragro.MVC.Helpers
{
    public static class FlashHelpers
    {
        private static string GetType(FlashEnum type)
        {
            return string.Format("flash-{0}", type.ToString().ToLower());
        }

        public static void Flash(
            this Controller controller, string message, FlashEnum type = FlashEnum.Success)
        {
            controller.TempData[GetType(type)] = message;
        }

        public static void Flash(
            this Controller controller, ModelStateDictionary modelState, FlashEnum type = FlashEnum.Success)
        {
            var error = new TagBuilder("ul");
            foreach (var ms in modelState)
            {
                if (ms.Value.Errors.Any())
                {
                    foreach (var err in ms.Value.Errors)
                    {
                        var li = new TagBuilder("li");
                        if (ms.Key.Trim().Length == 0)
                            li.InnerHtml = string.Format("The page has the error(s): \n\n {0}", err.ErrorMessage);
                        else
                            li.InnerHtml = string.Format("The property '{0}' has the error '{1}'", ms.Key, err.ErrorMessage);
                        error.InnerHtml += li.ToString();
                    }
                }
            }
            controller.TempData[GetType(type)] = error.ToString();
        }

        public static void Flash(
            this Controller controller, IList<RuleViolation> ruleViolations, FlashEnum type = FlashEnum.Success)
        {
            var error = new TagBuilder("ul");
            foreach (var rv in ruleViolations)
            {
                var li = new TagBuilder("li");
                li.InnerHtml = string.Format("The model has the error(s): \n\n '{0}'", rv.Message);
                error.InnerHtml += li.ToString();
            }
            controller.TempData[GetType(type)] = error.ToString();
        }

        public static MvcHtmlString Flash(this HtmlHelper helper)
        {
            var tempData = helper.ViewContext.TempData;

            var flash = tempData.Where(item => item.Key.StartsWith("flash-"))
                .Select(item =>
                    new { Message = item.Value, ClassName = item.Key }).FirstOrDefault();

            var sb = new StringBuilder();
            if (flash != null)
            {
                sb.AppendLine("<script type=\"text/javascript\">");
                sb.AppendLine("\t$(function () {");
                sb.AppendLine("\t\tvar $flash = $('<div id=\"flash\" style=\"display:none;\">');");
                sb.AppendLine(string.Format("\t\t$flash.html(\"{0}\");", MvcHtmlString.Create(flash.Message.ToString().Replace("\n", "<br />").Replace("\r", "<br />").Replace(Environment.NewLine, "<br />"))));
                sb.AppendLine("\t\t$flash.toggleClass('flash');");
                sb.AppendLine(string.Format("\t\t$flash.toggleClass('{0}');", flash.ClassName));
                sb.AppendLine("\t\t$('body').prepend($flash);");
                sb.AppendLine("\t\t$flash.slideDown('slow');");
                sb.AppendLine("\t\t$flash.click(function () { $(this).slideToggle('highlight'); });");
                sb.AppendLine("\t});");
                sb.AppendLine("\t</script>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
