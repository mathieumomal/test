using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;

namespace Etsi.Ultimate.Module.Specifications
{
    /// <summary>
    /// Base Page for Specification View / Edit Pages
    /// </summary>
    public class SpecificationBasePage : System.Web.UI.Page
    {
        #region Constants

        private const string CONST_VERSION_REMARKS = "VersionRemarks";
        private const string CONST_SPEC_RELEASE_REMARKS = "SpecReleaseRemarks";
        private const string CONST_SPEC_RELEASE_RIGHTS = "SpecReleaseRights";

        #endregion

        #region Public Properties

        /// <summary>
        /// Version Remarks - Stored in ViewState to access from child user controls
        /// </summary>
        public List<KeyValuePair<int, List<Remark>>> VersionRemarks
        {
            get
            {
                if (ViewState[CONST_VERSION_REMARKS] == null)
                    ViewState[CONST_VERSION_REMARKS] = new List<KeyValuePair<int, List<Remark>>>();

                return (List<KeyValuePair<int, List<Remark>>>)ViewState[CONST_VERSION_REMARKS];
            }
        }

        /// <summary>
        /// Specification Release Remarks - Stored in ViewState to access from child user controls
        /// </summary>
        public List<KeyValuePair<int, List<Remark>>> SpecReleaseRemarks
        {
            get
            {
                if (ViewState[CONST_SPEC_RELEASE_REMARKS] == null)
                    ViewState[CONST_SPEC_RELEASE_REMARKS] = new List<KeyValuePair<int, List<Remark>>>();

                return (List<KeyValuePair<int, List<Remark>>>)ViewState[CONST_SPEC_RELEASE_REMARKS];
            }
        }

        /// <summary>
        /// Specification Release Rights - Stored in ViewState to access from child user controls
        /// </summary>
        public List<KeyValuePair<int, UserRightsContainer>> SpecReleaseRights
        {
            get
            {
                if (ViewState[CONST_SPEC_RELEASE_RIGHTS] == null)
                    ViewState[CONST_SPEC_RELEASE_RIGHTS] = new List<KeyValuePair<int, UserRightsContainer>>();

                return (List<KeyValuePair<int, UserRightsContainer>>)ViewState[CONST_SPEC_RELEASE_RIGHTS];
            }
        }

        #endregion
    }
}