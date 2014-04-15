using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Etsi.Ultimate.Services;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Client;
using Telerik.Web.UI;
using System.Data;

namespace Etsi.Ultimate.Controls
{
    public partial class RapporteurControl : System.Web.UI.UserControl
    {

        #region constants
        private const string CONST_RAPPORTEURS_DATASOURCE = "RapporteursDataSource";
        private const string CONST_RAPPORTEURS_ID = "RapporteursIds"; 
        #endregion

        #region public properties
        public Boolean isPrimarySelectable { get; set; }
        public Boolean isMultiple { get; set; }
        public List<int> listIdPersonsSelect 
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_ID] == null)
                    ViewState[CONST_RAPPORTEURS_ID] = new List<int>()
                    {
                        14,23,25,10,6
                    };

                return (List<int>)ViewState[CONST_RAPPORTEURS_ID];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_ID] = value;
                RefreshDataSourceFromListIds();
            }
        }
        public List<View_Persons> DataSource
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_DATASOURCE] == null)
                    ViewState[CONST_RAPPORTEURS_DATASOURCE] = new List<View_Persons>();

                return (List<View_Persons>)ViewState[CONST_RAPPORTEURS_DATASOURCE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_DATASOURCE] = value;
                RefreshRadGrid();
            }
        }
        #endregion

        #region private properties
        
        #endregion

        #region events
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshDataSourceFromListIds();
        }

        //Table

        protected void RdGridRapporteurs_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int ID = GetSelectedPersonId(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PERSON_ID"].ToString());
            List<int> temp = new List<int>();
            foreach (int rapporteurId in listIdPersonsSelect.ToList())
            {
                if (rapporteurId != ID)
                {
                    temp.Add(rapporteurId);
                }
            }
            listIdPersonsSelect = temp;
        }

        //Search
        protected void RdcbRapporteurs_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                var svc = ServicesFactory.Resolve<IPersonService>();
                var personsFound = svc.LookFor(e.Text);
                BindDropDownData(personsFound);
            }
        }

        protected void BtnAddRapporteur_onClick(object sender, EventArgs e)
        {
            var idPersonSelected = GetSelectedPersonId(rdcbRapporteurs.SelectedValue);
            if (!listIdPersonsSelect.Contains(idPersonSelected))
            {
                listIdPersonsSelect.Add(idPersonSelected);
                RefreshDataSourceFromListIds();
            }
            else
            {
                launchAlert("This person is already in the list.");
            }
        }
        #endregion

        #region private methods
        private void BindDropDownData(List<View_Persons> personsList)
        {
            rdcbRapporteurs.DataSource = personsList;
            rdcbRapporteurs.DataTextField = "PersonSearchTxt";
            rdcbRapporteurs.DataValueField = "PERSON_ID";
            rdcbRapporteurs.DataBind();
        }

        private void RefreshDataSourceFromListIds()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            DataSource = personService.GetByIds(listIdPersonsSelect);
        }

        private void RefreshRadGrid()
        {
            rdGridRapporteurs.DataSource = DataSource;
            rdGridRapporteurs.Rebind();
        }

        private int GetSelectedPersonId(string rdcbValueSelected)
        {
            if (rdcbValueSelected.Length < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(rdcbValueSelected, out personID))
                    return personID;
            }
            return 0;
        }

        private void launchAlert(string errorText){
            RadWindowAlert.RadAlert(errorText, 400, 150, "Error", "", "images/error.png");
        }
        #endregion
    }
}