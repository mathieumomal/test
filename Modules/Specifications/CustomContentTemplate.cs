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
        private UserRightsContainer _releaseRights;
        private Page _page;
        private int? _personId;
        private int? _specId;
        private int? _releaseId;
        private bool _isEditMode;
        public CustomContentTemplate(List<SpecVersion> specVersions, UserRightsContainer releaseRights, int personId, int specId, int releaseId, bool isEditMode, Page page)
        {
            _specVersions = specVersions;
            _releaseRights = releaseRights;
            _page = page;
            _personId = personId;
            _specId = specId;
            _releaseId = releaseId;
            _isEditMode = isEditMode;
        }

        public void InstantiateIn(Control container)
        {
            SpecificationVersionListControl ctrl = (SpecificationVersionListControl)_page.LoadControl("SpecificationVersionListControl.ascx");
            if (ctrl != null)
            {
                ctrl.DataSource = _specVersions;
                ctrl.UserReleaseRights = _releaseRights;
                ctrl.PersonId = _personId;
                ctrl.SpecId = _specId;
                ctrl.ReleaseId = _releaseId;
                ctrl.IsEditMode = _isEditMode;
            }
            container.Controls.Add(ctrl);
        }
    }
}