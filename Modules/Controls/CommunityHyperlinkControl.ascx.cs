using System;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Controls
{
    
    public partial class CommunityHyperlinkControl : System.Web.UI.UserControl
    {
        #region constants
        public const string DefaultNoDataValue = " - ";
        public const string DefaultCommunityNameType = "FULL";
        public const string NoDataValueViewState = "NoDataValueViewState";
        #endregion

        #region Public properties

        public List<int> CommunityIds { get; set; }

        public string NoDataValue
        {
            get
            {
                if (ViewState[ClientID + NoDataValueViewState] == null)
                    ViewState[ClientID + NoDataValueViewState] = "";
                return (string)ViewState[ClientID + NoDataValueViewState];
            }
            set
            {
                ViewState[ClientID + NoDataValueViewState] = value;
            }
        }

        public string CommunityNameType { get; set; }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshControl(CommunityIds, CommunityNameType, NoDataValue);
            }
        }

        public void RefreshControl(List<int> communityIds, string communityNameType = DefaultCommunityNameType, string noDataValue = DefaultNoDataValue)
        {
            CommunityIds = communityIds;
            CommunityNameType = communityNameType;
            NoDataValue = noDataValue;

            HideEverything();
            SetNoDataValue();
            SetCommunityNameType();
            DisplayCommunities();
        }
        #endregion

        #region Private methods

        private void DisplayCommunities()
        {
            if (CommunityIds == null || CommunityIds.Count == 0)
            {
                SetNoDataValue();
                lblNoData.Visible = true;
            }
            else
            {
                var service = ServicesFactory.Resolve<ICommunityService>();
                var communities = service.GetCommunitiesByIds(CommunityIds);

                if (communities == null)
                {
                    lblError.Visible = true;
                    return;
                }

                //Create correct facade object
                var finalCommunityList = new List<CommunityFacade>();
                foreach (var com in communities)
                {
                    var finalName = string.Empty;
                    if (CommunityNameType.ToUpper().Trim() == EnumCommunityNameType.FULL.ToString().ToUpper().Trim())
                    {
                        finalName = com.TbName;
                    }
                    else if (CommunityNameType.ToUpper().Trim() == EnumCommunityNameType.SHORT.ToString().ToUpper().Trim())
                    {
                        finalName = com.ShortName;
                    }
                    else if (CommunityNameType.ToUpper().Trim() == EnumCommunityNameType.FULL_WITHOUT_3GPP.ToString().ToUpper().Trim())
                    {
                        finalName = com.TbName.Remove3GppAtTheBeginningOfAString();
                    }

                    finalCommunityList.Add(new CommunityFacade
                    {
                        Id = com.TbId,
                        Name = finalName,
                        DetailsUrl = com.DetailsURL,
                        Tooltip = string.Format("{0} - {1}", finalName, com.DetailsURL)
                    });
                }

                rptMultipleCommunities.DataSource = finalCommunityList;
                rptMultipleCommunities.DataBind();

                rptMultipleCommunities.Visible = true;
            }
        }

        private void HideEverything()
        {
            rptMultipleCommunities.Visible = false;
            lblNoData.Visible = false;
            lblError.Visible = false;
        }

        private void SetNoDataValue()
        {
            if(string.IsNullOrEmpty(NoDataValue))
                NoDataValue = DefaultNoDataValue;
        }

        private void SetCommunityNameType()
        {
            if (string.IsNullOrEmpty(CommunityNameType)  || 
                (CommunityNameType.ToUpper().Trim() != EnumCommunityNameType.FULL.ToString().ToUpper().Trim()
                && CommunityNameType.ToUpper().Trim() != EnumCommunityNameType.SHORT.ToString().ToUpper().Trim()
                && CommunityNameType.ToUpper().Trim() != EnumCommunityNameType.FULL_WITHOUT_3GPP.ToString().ToUpper().Trim()))
                CommunityNameType = DefaultCommunityNameType;
        }
        #endregion
    }

    public enum EnumCommunityNameType
    {
        FULL,
        SHORT,
        FULL_WITHOUT_3GPP
    }
}