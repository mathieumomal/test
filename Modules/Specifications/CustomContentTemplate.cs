using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;
using System.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    /// <summary>
    /// Custom Template to render Versions Grid on Specification Releases Tab
    /// </summary>
    public class CustomContentTemplate : ITemplate
    {
        #region Private Variables

        private Page _page;
        private int? _personId;
        private bool _isEditMode;
        private double _scrollHeight;
        private List<SpecVersion> _versions;
        private Specification_Release _specRelease;
        private bool _isSpecNumberAssigned;
        private AdditionalVersionInfo _specDecorator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Custom Template for Versions Grid
        /// </summary>
        /// <param name="specRelease">Specification Release</param>
        /// <param name="versions">Versions</param>
        /// <param name="isEditMode">True - Edit Mode / False - View Mode</param>
        /// <param name="personId">Person ID</param>
        /// <param name="page">Page</param>
        /// <param name="scrollHeight">Scroll Height</param>
        public CustomContentTemplate(bool isSpecNumberAssigned, Specification_Release specRelease, List<SpecVersion> versions, AdditionalVersionInfo specDecorator, bool isEditMode, int personId, Page page, double scrollHeight)
        {
            _specRelease = specRelease;
            _versions = versions;
            _page = page;
            _personId = personId;
            _isEditMode = isEditMode;
            _scrollHeight = scrollHeight;
            _isSpecNumberAssigned = isSpecNumberAssigned;
            _specDecorator = specDecorator;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Instanciate controls on Specification Release Tab
        /// </summary>
        /// <param name="container">Controls container</param>
        public void InstantiateIn(Control container)
        {
            SpecificationVersionListControl ctrl = (SpecificationVersionListControl)_page.LoadControl("SpecificationVersionListControl.ascx");
            if (ctrl != null)
            {
                ctrl.SpecReleaseID = _specRelease.Pk_Specification_ReleaseId;
                ctrl.Versions = _versions;
                ctrl.PersonId = _personId;
                ctrl.SpecId = _specRelease.Fk_SpecificationId;
                ctrl.ReleaseId = _specRelease.Fk_ReleaseId;
                ctrl.IsEditMode = _isEditMode;
                ctrl.ScrollHeight = _scrollHeight;
                ctrl.IsSpecNumberAssigned = _isSpecNumberAssigned;
                ctrl.AdditionalVersionInfo = _specDecorator;
            }
            container.Controls.Add(ctrl);
        }

        #endregion
    }
}