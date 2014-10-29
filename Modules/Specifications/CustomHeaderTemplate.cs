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

        private Page _page;
        private bool _isEditMode;
        private Specification_Release _specRelease;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for Header Template
        /// </summary>
        /// <param name="specRelease">Specification Release</param>
        /// <param name="isEditMode">True - Edit Mode / False - View Mode</param>
        /// <param name="page">Page</param>
        public CustomHeaderTemplate(Specification_Release specRelease, bool isEditMode, Page page)
        {
            _specRelease = specRelease;
            _isEditMode = isEditMode;
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
            }
            container.Controls.Add(ctrl);
        }

        #endregion
    }
}