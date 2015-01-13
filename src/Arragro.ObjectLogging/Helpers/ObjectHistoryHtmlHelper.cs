using Arragro.ObjectLogging;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Arragro.ObjectLogging.Helpers
{
    public static class ObjectHistoryHtmlHelper
    {
        private static IEnumerable<ObjectHistory> GetObjectHistories<TModel, TKeyType>(UnityContainer unityContainer, TModel obj)
        {
            var keyId= Arragro.Common.Helpers.ObjectHelpers.GetKeyPropertyValue<TModel, TKeyType>(obj);
            var objectHistories = unityContainer.Resolve<Arragro.ObjectLogging.Interfaces.IObjectHistoryRepository>();
            var key = obj.GetType().Name + ":" + keyId.ToString();

            return objectHistories.All()
                        .Where(o => o.Key == key)
                        .OrderBy(o => o.DateChanged)
                        .ToList();
        }

        private static IEnumerable<ObjectHistory> GetObjectHistoryData<TModel, TKeyType>(HtmlHelper<TModel> htmlHelper, UnityContainer unityContainer)
        {
            var model = htmlHelper.ViewData.Model;
            return GetObjectHistories<TModel, TKeyType>(unityContainer, model);
        }

        private static string BuildTableRows(ObjectHistory objectHistory)
        {
            var sb = new StringBuilder();
            const string template = @"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                            </tr>";

            foreach (var cr in objectHistory.ComparisonResults)
            {
                sb.AppendLine(string.Format(template, cr.Name, cr.OriginalValue, cr.NewValue));
            }
            return sb.ToString();
        }

        public static MvcHtmlString GetObjectHistories<TModel, TKeyType>(this HtmlHelper<TModel> htmlHelper, UnityContainer unityContainer)
        {
            var objectHistories = GetObjectHistoryData<TModel, TKeyType>(htmlHelper, unityContainer);

            var div = new TagBuilder("div");
            var count = 1;
            foreach(var oh in objectHistories)
            {
                const string template = @"
<ul class='list-unstyled'>
    <li data-toggle='collapse' data-target='#collapse{3}'><strong>{0} - {1}</strong></li>
    <li id='collapse{3}' class='collapse' aria-expanded='false' aria-controls='collapse{3}'>
        <ul class='list-unstyled'>
            <li>
                <table class='table table-bordered table-striped table-condensed table-responsive table-hover'>
                    <thead>
                        <tr>
                            <th>Property</th>
                            <th>Old Value</th>
                            <th>New Value</th>
                        </tr>
                    </thead>
                    <tbody>
                        {2}
                    </tbody>
                </table>
            </li>
        </ul>
    </li>
</ul>";
                
                div.InnerHtml += string.Format(template, oh.DateChanged.ToString("dd/MM/yyyy HH:mm:ss"), oh.UserName, BuildTableRows(oh), count);
                count++;
            }

            return MvcHtmlString.Create(div.ToString());
        }
    }
}