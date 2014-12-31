using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using Etsi.Ultimate.Services;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.CRs
{
    /// <summary>
    /// Summary description for GetWiMtgData
    /// </summary>
    [WebService(Namespace = "http://portal.3gpp.org/ws")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class GetWiMtgData : WebService
    {
        private const int MaxResults = 50;

        [WebMethod]
        public AutoCompleteBoxData GetWorkItems(RadAutoCompleteContext context)
        {
            var searchString = context["Text"].ToString();
            var wiSvc = ServicesFactory.Resolve<IWorkItemService>();
            var wis = wiSvc.GetWorkItemsBySearchCriteria(0, searchString).Key.Take(MaxResults).ToList();
            
            var result = new List<AutoCompleteBoxItemData>();

            foreach (var wi in wis)
            {
                var childNode = new AutoCompleteBoxItemData
                {
                    Text = wi.WorkItemDdlText,
                    Value = wi.Pk_WorkItemUid.ToString(CultureInfo.InvariantCulture)
                };
                result.Add(childNode);
            }

            var res = new AutoCompleteBoxData {Items = result.ToArray()};

            return res;
        }

        [WebMethod]
        public AutoCompleteBoxData GetMeetings(RadAutoCompleteContext context)
        {
            var searchString = context["Text"].ToString();
            var mtgSvc = ServicesFactory.Resolve<IMeetingService>();
            var meetings = mtgSvc.GetMatchingMeetings(searchString).Take(MaxResults).ToList();

            var result = new List<AutoCompleteBoxItemData>();

            foreach (var meeting in meetings)
            {
                var mtgText = meeting.MtgDdlText;
                var childNode = new AutoCompleteBoxItemData
                {
                    Text = mtgText,
                    Value = meeting.MTG_ID.ToString(CultureInfo.InvariantCulture)
                };
                result.Add(childNode);
            }

            var res = new AutoCompleteBoxData {Items = result.ToArray()};

            return res;
        }
    }
}
