using Etsi.Ultimate.DomainClasses;
using System.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    /// <summary>
    /// Custom Template to render header for Specification Releases
    /// </summary>
    public class CustomHeaderTemplate : ITemplate
    {
        #region Private Methods

        private Release _release;
        private Specification _specification;
        private Page _page;
        private bool _isEditMode;
        private UserRightsContainer _releaseRights;
        private int? _personId;
        private Specification_Release _specRelease;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for Header Template
        /// </summary>
        /// <param name="specRelease">Specification Release</param>
        /// <param name="isEditMode">True - Edit Mode / False - View Mode</param>
        /// <param name="personId">Person ID</param>
        /// <param name="page">Page</param>
        public CustomHeaderTemplate(Specification_Release specRelease, bool isEditMode, int personId, Page page)
        {
            _specRelease = specRelease;
            _isEditMode = isEditMode;
            _personId = personId;
            _page = page;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Instanciate header control on Specification release tab
        /// </summary>
        /// <param name="container">Controls Container</param>
        public void InstantiateIn(Control container)
        {
            ReleaseHeaderControl ctrl = (ReleaseHeaderControl)_page.LoadControl("ReleaseHeaderControl.ascx");
            if (ctrl != null)
            {
                ctrl.SpecRelease = _specRelease;
                ctrl.IsEditMode = _isEditMode;
                ctrl.PersonId = _personId;
            }
            container.Controls.Add(ctrl);
        }

        #endregion
    }
}