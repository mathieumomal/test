using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public class CustomContentTemplate : ITemplate
    {
        private List<SpecVersion> _specVersions;
        private Page _page;
        public CustomContentTemplate(List<SpecVersion> specVersions, Page page)
        {
            _specVersions = specVersions;
            _page = page;
        }

        public void InstantiateIn(Control container)
        {
            SpecificationVersionListControl ctrl = (SpecificationVersionListControl)_page.LoadControl("SpecificationVersionListControl.ascx");
            if (ctrl != null)
            {
                ctrl.DataSource = _specVersions;
            }
            container.Controls.Add(ctrl);
        }
    }
}