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
    public class CustomHeaderTemplate : ITemplate
    {
        private Release _release;
        private Specification _specification;
        private Page _page;
        public CustomHeaderTemplate(Release release, Specification specification, Page page)
        {
            _specification = specification;
            _release = release;
            _page = page;
        }

        public void InstantiateIn(Control container)
        {


            ReleaseHeaderControl ctrl = (ReleaseHeaderControl)_page.LoadControl("ReleaseHeaderControl.ascx");
            if (ctrl != null)
            {
                ctrl.ReleaseDataSource = _release;
                ctrl.SpecificationDataSource = _specification;
            }
            container.Controls.Add(ctrl);
        }
    }
}