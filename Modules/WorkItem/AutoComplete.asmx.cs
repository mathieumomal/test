using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Module.WorkItem
{
    /// <summary>
    /// Description résumée de AutoComplete
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    [System.Web.Script.Services.ScriptService]
    public class AutoComplete : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] GetAcronyms(string prefixText, int count)
        {
            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            return wiService.LookForAcronyms(prefixText).ToArray();
        }
    }
}
