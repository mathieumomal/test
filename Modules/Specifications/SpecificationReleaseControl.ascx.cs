using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationReleaseControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_DATASOURCE = "SpecificationReleaseControl_datasource";
        private const string CONST_IS_EDIT_MODE = "SpecificationReleaseControl_isEditMode";
        private const string CONST_PERSON_ID = "SpecificationReleaseControl_personId";

        #endregion

        #region Public Properties

        public int? PersonId
        {
            get
            {
                return (int?)ViewState[CONST_PERSON_ID];
            }
            set
            {
                ViewState[CONST_PERSON_ID] = value;
            }
        }
        public bool IsEditMode
        {
            get
            {
                return ((bool?)ViewState[CONST_IS_EDIT_MODE]).GetValueOrDefault();
            }
            set
            {
                ViewState[CONST_IS_EDIT_MODE] = value;
            }
        }

        public Specification DataSource
        {
            get
            {
                if (ViewState[CONST_DATASOURCE] == null)
                    return new Specification();
                else
                    return (Specification)ViewState[CONST_DATASOURCE];
            }
            set
            {
                ViewState[CONST_DATASOURCE] = GetActualSpecificationFromProxy(value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Specification Release Control
        /// </summary>
        /// <param name="sender">Specification Release Control</param>
        /// <param name="e">Page load event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Load this control only in EditMode & when it has atleast one spec-release mapping
            if ((Request.QueryString["action"] == null || Request.QueryString["action"] != "create") && (DataSource.SpecificationReleases != null && DataSource.SpecificationReleases.Count > 0))
            {
                var additionalVersionInfo = new AdditionalVersionInfo();
                if (!IsPostBack)
                {
                    foreach (var release in DataSource.SpecificationReleases.OrderByDescending(sr => sr.SortOrder))
                    {
                        var item = new RadPanelItem
                        {
                            Value = release.Pk_ReleaseId.ToString()
                        };
                        rpbReleases.Items.Add(item);
                    }

                    // Get the rights of the user
                    var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    var userRightsPerSpecRelease = specSvc.GetRightsForSpecReleases(PersonId.GetValueOrDefault(), DataSource);

                    //Get spec decorator data :
                        //Get versions foundations CRs data
                    additionalVersionInfo.SpecVersionFoundationCrs =
                        specSvc.GetSpecVersionsFoundationCrs(PersonId.GetValueOrDefault(), DataSource.Pk_SpecificationId).Result;

                    var basePage = (SpecificationBasePage)this.Page;
                    basePage.SpecReleaseRights.Clear();
                    userRightsPerSpecRelease.ForEach( x => basePage.SpecReleaseRights.Add(new KeyValuePair<int,UserRightsContainer>(x.Key.Pk_Specification_ReleaseId, x.Value)));
                }

                double panelHeight = rpbReleases.Height.Value;
                double panelItemsHeaderHeight = (rpbReleases.Items.Count) * 31;
                double iconsHeight = 40;
                double gridHeaderHeight = 30;
                double padding = 15;
                double scrollHeight = panelHeight - panelItemsHeaderHeight - iconsHeight - gridHeaderHeight - padding;

                //Dynamic Header & Content controls always needs to re-create for each postback
                foreach (RadPanelItem item in rpbReleases.Items)
                {
                    var release = DataSource.SpecificationReleases.Where(x => x.Pk_ReleaseId.ToString() == item.Value).FirstOrDefault();
                    var versions = DataSource.Versions.Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == release.Pk_ReleaseId : false)
                                                               .OrderByDescending(x => x.MajorVersion)
                                                               .ThenByDescending(x => x.TechnicalVersion)
                                                               .ThenByDescending(x => x.EditorialVersion).ToList();

                    //We find if the spec number is define to display or not the Allocate and Upload buttons :
                    var isSpecNumberAssigned = true;
                    if (String.IsNullOrEmpty(DataSource.Number))
                        isSpecNumberAssigned = false;

                    var specRelease = DataSource.Specification_Release.Where(x => x.Fk_ReleaseId.ToString() == item.Value && x.Fk_SpecificationId == DataSource.Pk_SpecificationId).FirstOrDefault();
                    var customHeaderTemplate = new CustomHeaderTemplate(specRelease, IsEditMode, this.Page);
                    var customContentTemplate = new CustomContentTemplate(isSpecNumberAssigned, specRelease, versions, additionalVersionInfo, IsEditMode, PersonId.GetValueOrDefault(), this.Page, scrollHeight);
                    item.HeaderTemplate = customHeaderTemplate;
                    item.ApplyHeaderTemplate();
                    item.ContentTemplate = customContentTemplate;
                    customContentTemplate.InstantiateIn(item);
                    item.Expanded = true;
                    item.DataBind();
                    item.ChildGroupHeight = 0;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Provide the required specification properties for release tab
        /// </summary>
        /// <param name="proxySpecification">Specification</param>
        /// <returns>Specification with required properties</returns>
        private Specification GetActualSpecificationFromProxy(Specification proxySpecification)
        {
            Specification specification = new Specification()
            {
                Pk_SpecificationId = proxySpecification.Pk_SpecificationId,
                IsActive = proxySpecification.IsActive,
                IsUnderChangeControl = proxySpecification.IsUnderChangeControl,
                promoteInhibited = proxySpecification.promoteInhibited,
                Number = proxySpecification.Number
            };
            
            if(proxySpecification.SpecificationReleases != null)
            {
                List<Release> specReleases = new List<Release>();
                proxySpecification.SpecificationReleases.ForEach(x => specReleases.Add(new Release() { Pk_ReleaseId = x.Pk_ReleaseId, SortOrder = x.SortOrder }));
                specification.SpecificationReleases = specReleases;
            }

            proxySpecification.Specification_Release.ToList().ForEach(x => specification.Specification_Release.Add(new Specification_Release()
            {
                Pk_Specification_ReleaseId = x.Pk_Specification_ReleaseId,
                Fk_SpecificationId = x.Fk_SpecificationId,
                Fk_ReleaseId = x.Fk_ReleaseId,
                isWithdrawn = x.isWithdrawn,
                WithdrawMeetingId = x.WithdrawMeetingId,
                isTranpositionForced = x.isTranpositionForced,
                CreationDate = x.CreationDate,
                UpdateDate = x.UpdateDate,
                Release = (x.Release == null) ? null : new Release() { Pk_ReleaseId = x.Release.Pk_ReleaseId, Name = x.Release.Name, SortOrder = x.Release.SortOrder },
                Specification = (x.Specification == null) ? null : new Specification() { Pk_SpecificationId = x.Specification.Pk_SpecificationId, IsActive = x.Specification.IsActive, IsUnderChangeControl = x.Specification.IsUnderChangeControl },
                Remarks = GetActualRemarksFromProxy(x.Remarks.ToList())
            }));

            proxySpecification.Versions.ToList().ForEach(x => specification.Versions.Add(new SpecVersion()
            {
                Pk_VersionId = x.Pk_VersionId,
                MajorVersion = x.MajorVersion,
                TechnicalVersion = x.TechnicalVersion,
                EditorialVersion = x.EditorialVersion,
                AchievedDate = x.AchievedDate,
                ExpertProvided = x.ExpertProvided,
                Location = x.Location,
                SupressFromSDO_Pub = x.SupressFromSDO_Pub,
                ForcePublication = x.ForcePublication,
                DocumentUploaded = x.DocumentUploaded,
                DocumentPassedToPub = x.DocumentPassedToPub,
                Multifile = x.Multifile,
                Source = x.Source,
                ETSI_WKI_ID = x.ETSI_WKI_ID,
                ProvidedBy = x.ProvidedBy,
                Fk_SpecificationId = x.Fk_SpecificationId,
                Fk_ReleaseId = x.Fk_ReleaseId,
                ETSI_WKI_Ref = x.ETSI_WKI_Ref,
                RelatedTDoc = x.RelatedTDoc,
                Remarks = GetActualRemarksFromProxy(x.Remarks.ToList())
            }));

            proxySpecification.SpecificationRapporteurs.ToList().ForEach(x => specification.SpecificationRapporteurs.Add(new SpecificationRapporteur()
            {
                Pk_SpecificationRapporteurId = x.Pk_SpecificationRapporteurId,
                Fk_SpecificationId = x.Fk_SpecificationId,
                Fk_RapporteurId = x.Fk_RapporteurId,
                IsPrime = x.IsPrime
            }));

            return specification;
        }

        /// <summary>
        /// Provide the required remark properties for release tab
        /// </summary>
        /// <param name="proxyRemarks">Remark</param>
        /// <returns>Remark with required properties</returns>
        private List<Remark> GetActualRemarksFromProxy(List<Remark> proxyRemarks)
        {
            List<Remark> remarks = new List<Remark>();
            proxyRemarks.ForEach(x => remarks.Add(new Remark()
            {
                Pk_RemarkId = x.Pk_RemarkId,
                Fk_PersonId = x.Fk_PersonId,
                IsPublic = x.IsPublic,
                CreationDate = x.CreationDate,
                RemarkText = x.RemarkText,
                PersonName = x.PersonName,
                Fk_SpecificationReleaseId = x.Fk_SpecificationReleaseId,
                Fk_VersionId = x.Fk_VersionId
            }));
            return remarks;
        }

        #endregion
    }
}