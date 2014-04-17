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
using System.Text;

namespace Etsi.Ultimate.Controls
{
    public partial class RapporteurControl : System.Web.UI.UserControl
    {

        #region constants
        //ViewStates
        private const string CONST_RAPPORTEURS_VIEWSTATE_DATASOURCE = "RapporteursDataSource";
        private const string CONST_RAPPORTEURS_VIEWSTATE_ID = "RapporteursIds";
        private const string CONST_RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE = "IsSinglePersonMode";
        private const string CONST_RAPPORTEURS_VIEWSTATE_ISEDITMODE = "IsEditMode";
        private const string CONST_RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE = "IdPersonSelected_SINGLEMODE";
        private const string CONST_RAPPORTEURS_VIEWSTATE_SELECTABLEMODE = "SelectableMode";
        private const string CONST_RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED = "ListIdPersonSelect";

        //Columns
        private const string CONST_RAPPORTEURS_COLUMN_SELECTABLE = "selectable";
        private const string CONST_RAPPORTEURS_COLUMN_DELETE = "delete";
        private const string CONST_RAPPORTEURS_COLUMN_EMAIL = "email";
        private const string CONST_RAPPORTEURS_COLUMN_NAME = "name";
        private const string CONST_RAPPORTEURS_COLUMN_NAMEHYPERLINK = "nameHyperLink";

        //CssClass
        private const string CONST_RAPPORTEURS_CSSCLASS_EDITMODE_ONEPERSONMODE = "editModeOnePersonMode";
        private const string CONST_RAPPORTEURS_CSSCLASS_VIEWMODE_ONEPERSONMODE = "viewModeOnePersonMode";

        private const string CONST_RAPPORTEURS_CSSCLASS_EDITMODE_MULTIPERSONMODE = "editModeMultiPersonMode";
        private const string CONST_RAPPORTEURS_CSSCLASS_VIEWMODE_MULTIPERSONMODE = "viewModeMultiPersonMode";

        public enum CONST_RAPPORTEURS_SELECTABLEMODE{
            multi,
            simple,
            none
        }
        #endregion

        #region public properties

        #region --- Modes ---
        /// <summary>
        /// Provide the edit current mode ; EDIT MODE (true) or VIEW MODE (false)
        /// </summary>
        public Boolean IsEditMode
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISEDITMODE] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISEDITMODE] = false;

                return (Boolean)ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISEDITMODE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISEDITMODE] = value;
            }
        }

        /// <summary>
        /// One person could be selected (isOneMode = true) or multiple persons could be selected (isOneMode = false)
        /// </summary>
        public Boolean IsSinglePersonMode
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] = false;

                return (Boolean)ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] = value;
            }
        }
        #endregion

        #region--- 'Selectabled' attributes ---

        /// <summary>
        /// This module provides a selection system
        /// </summary>
        public string SelectableMode 
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] = "none";

                return (String)ViewState[CONST_RAPPORTEURS_VIEWSTATE_SELECTABLEMODE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] = value;
            }
        }

        /// <summary>
        /// List of selected persons
        /// </summary>
        public List<int> ListIdPersonSelect 
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] = new List<int>();

                return (List<int>)ViewState[CONST_RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] = value;
            }
        }

        #endregion

        #region--- Single mode attribute ---

        /// <summary>
        /// For the Single selected mode, this attribute provides the last person's id choosen
        /// </summary>
        public int IdPersonSelected_SINGLEMODE {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] = 0;

                return (int)ViewState[CONST_RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] = value;
            }
        }
        #endregion

        #region--- Multiple mode attributes ---

        /// <summary>
        /// List of choosen person's id
        /// </summary>
        public List<int> ListIdPersonsSelected_MULTIMODE 
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_ID] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_ID] = new List<int>();

                return (List<int>)ViewState[CONST_RAPPORTEURS_VIEWSTATE_ID];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_ID] = value;
            }
        }
        /// <summary>
        /// Datasource for the radgrid which contains the list of persons (MODELS)
        /// </summary>
        public List<View_Persons> DataSource_MULTIMODE
        {
            get
            {
                if (ViewState[CONST_RAPPORTEURS_VIEWSTATE_DATASOURCE] == null)
                    ViewState[CONST_RAPPORTEURS_VIEWSTATE_DATASOURCE] = new List<View_Persons>();

                return (List<View_Persons>)ViewState[CONST_RAPPORTEURS_VIEWSTATE_DATASOURCE];
            }
            set
            {
                ViewState[CONST_RAPPORTEURS_VIEWSTATE_DATASOURCE] = value;
            }
        }
        #endregion

        #endregion



        #region Events

        #region --- Init methods ---
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitControl();
            }
        }
        /*
        private void TempInitialize()
        {
            IsEditMode = true;
            //IdPersonSelected_SINGLEMODE = 14;
            IsSinglePersonMode = false;
            SelectableMode = CONST_RAPPORTEURS_SELECTABLEMODE.simple.ToString();
            ListIdPersonsSelected_MULTIMODE = new List<int>()
            {
                14,23,25,10,6
            };
        }
        */

        /// <summary>
        /// Initialization of control
        /// </summary>
        private void InitControl()
        {
            //TempInitialize();
            var selectableColumn = (GridClientSelectColumn)rdGridRapporteurs.MasterTableView.GetColumn(CONST_RAPPORTEURS_COLUMN_SELECTABLE);
            var deletedColumn = (GridButtonColumn)rdGridRapporteurs.MasterTableView.GetColumn(CONST_RAPPORTEURS_COLUMN_DELETE);
            var emailColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(CONST_RAPPORTEURS_COLUMN_EMAIL);
            var nameColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(CONST_RAPPORTEURS_COLUMN_NAME);
            var nameHyperLinkColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(CONST_RAPPORTEURS_COLUMN_NAMEHYPERLINK);
            if (IsEditMode)//EDIT MODE CONFIG
            {
                #region Selection system manager
                if (SelectableMode.Equals(CONST_RAPPORTEURS_SELECTABLEMODE.simple.ToString()))
                {
                    selectableColumn.Visible = true;
                    rdGridRapporteurs.AllowMultiRowSelection = false;
                    rdGridRapporteurs.ClientSettings.EnablePostBackOnRowClick = true;
                }
                else if (SelectableMode.Equals(CONST_RAPPORTEURS_SELECTABLEMODE.multi.ToString()))
                {
                    selectableColumn.Visible = true;
                    rdGridRapporteurs.AllowMultiRowSelection = true;
                    rdGridRapporteurs.ClientSettings.EnablePostBackOnRowClick = true;
                }
                #endregion

                #region Single or multi mode manager
                if (!IsSinglePersonMode)
                {
                    panelRapporteurControl.CssClass = CONST_RAPPORTEURS_CSSCLASS_EDITMODE_MULTIPERSONMODE;
                    RefreshDataSourceFromListIds_MULTIMODE();
                    RefreshDisplay_MULTIMODE();
                }
                else
                {
                    panelRapporteurControl.CssClass = CONST_RAPPORTEURS_CSSCLASS_EDITMODE_ONEPERSONMODE;
                    rdGridRapporteurs.Visible = false;
                    RefreshDisplay_SINGLEMODE();
                }
                #endregion
            }
            else//VIEW MODE CONFIG
            {
                #region Single or multi mode manager
                if (!IsSinglePersonMode)//MULTI PERSON MODE
                {
                    panelRapporteurControl.CssClass = CONST_RAPPORTEURS_CSSCLASS_VIEWMODE_MULTIPERSONMODE;

                    nameHyperLinkColumn.Visible = true;
                    nameColumn.Visible = false;

                    deletedColumn.Visible = false;
                    emailColumn.Visible = false;

                    btnAddRapporteur.Visible = false;
                    lblAddRapporteur.Visible = false;
                    rdcbRapporteurs.Visible = false;
                    RefreshDataSourceFromListIds_MULTIMODE();
                    RefreshDisplay_MULTIMODE();
                }
                else//ONE PERSON MODE
                {
                    panelRapporteurControl.CssClass = CONST_RAPPORTEURS_CSSCLASS_VIEWMODE_ONEPERSONMODE;

                    rdGridRapporteurs.Visible = false;

                    rdcbRapporteurs.Enabled = false;

                    btnAddRapporteur.Visible = false;
                    lblAddRapporteur.Visible = false;
                    RefreshDisplay_SINGLEMODE();
                }
                #endregion
            }
        }
        #endregion

        #region--- ADD/DELETE/SELECTABLE PART (MULTIPLE) ---
        /// <summary>
        /// Add button for the multiple mode
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void BtnAddRapporteur_onClick(object o, EventArgs e)
        {
            var personIdToAdd = GetIdSelected();
            if (personIdToAdd == 0)
            {
                LaunchAlert("Select someone to add to the list.");
            }
            else
            {
                if (!ListIdPersonsSelected_MULTIMODE.Contains(personIdToAdd))
                {
                    AddPerson_MULTIMODE(personIdToAdd);
                    rdcbRapporteurs.Text = "";
                    rdcbRapporteurs.ClearSelection();
                    RefreshDisplay_MULTIMODE();
                }
                else
                {
                    LaunchAlert("This person is already in the list.");
                }
            }
        }

        /// <summary>
        /// Delete someone from the radgrid (and by consequences to the list of ids)
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RdGridRapporteurs_DeleteCommand(object o, Telerik.Web.UI.GridCommandEventArgs e)
        {
            var ID = ConvertStringToInt(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PERSON_ID"].ToString());
            var temp = new List<int>();
            RemovePerson_MULTIMODE(ID);
            RefreshDisplay_MULTIMODE();
        }

        /// <summary>
        /// Method which provide the SELECTION system
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RdGridRapporteurs_SelectedIndexChanged(object o, EventArgs e)
        {
            var temp = new List<int>();
            foreach (GridDataItem item in   rdGridRapporteurs.MasterTableView.Items) 
            { 
                if (item.Selected)
                {
                    var ID = ConvertStringToInt(item.OwnerTableView.DataKeyValues[item.ItemIndex]["PERSON_ID"].ToString());
                    temp.Add(ID);
                }
            }
            ListIdPersonSelect = temp;
        }
        #endregion

        #region--- Search PART (SINGLE/MULTIPLE) ---

        /// <summary>
        /// Function run when we begin to write a keyword in the combobox
        /// Search a person corresponding to a keyword
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RdcbRapporteurs_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                var svc = ServicesFactory.Resolve<IPersonService>();
                var personsFound = svc.LookFor(e.Text);
                BindDropDownData(personsFound);
            }
        }

        /// <summary>
        /// Usefull for the single mode
        /// When we select a person (in single mode) we put the person's id to the IdPersonSelected_SINGLEMODE attribute
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RdcbRapporteurs_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (IsSinglePersonMode)
            {
                IdPersonSelected_SINGLEMODE = ConvertStringToInt(e.Value);
            }
        }
        #endregion

        #endregion



        #region private methods

        #region--- Search PART ---
        /// <summary>
        /// Binding for the search combobox
        /// </summary>
        /// <param name="personsList"></param>
        private void BindDropDownData(List<View_Persons> personsList)
        {
            rdcbRapporteurs.DataSource = personsList;
            rdcbRapporteurs.DataTextField = "PersonSearchTxt";
            rdcbRapporteurs.DataValueField = "PERSON_ID";
            rdcbRapporteurs.DataBind();
        }

        /// <summary>
        /// Get the current choosen person with the search combobox
        /// </summary>
        /// <returns></returns>
        private int GetIdSelected()
        {
            return ConvertStringToInt(rdcbRapporteurs.SelectedValue);
        }
        #endregion

        #region--- MULTIMODE PART ---
        /// <summary>
        /// Refresh the Datasource attribute thanks to the list of persons' ids (MULTIMODE)
        /// </summary>
        private void RefreshDataSourceFromListIds_MULTIMODE()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            DataSource_MULTIMODE = personService.GetByIds(ListIdPersonsSelected_MULTIMODE);
        }

        /// <summary>
        /// Refresh the Datasource by the last person added
        /// </summary>
        private void AddPerson_MULTIMODE(int id)
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            ListIdPersonsSelected_MULTIMODE.Add(id);
            DataSource_MULTIMODE.Add(personService.FindPerson(id));
        }

        /// <summary>
        /// Remove someone of the list
        /// </summary>
        /// <param name="id"></param>
        private void RemovePerson_MULTIMODE(int id)
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            ListIdPersonsSelected_MULTIMODE.Remove(id);
            DataSource_MULTIMODE.Remove(DataSource_MULTIMODE.SingleOrDefault( s => s.PERSON_ID == id));
        }

        /// <summary>
        /// Refresh radgrid (rebind)
        /// </summary>
        private void RefreshDisplay_MULTIMODE()
        {
            rdGridRapporteurs.DataSource = DataSource_MULTIMODE;
            rdGridRapporteurs.Rebind();
        }
        #endregion

        #region--- SINGLEMODE PART ---
        /// <summary>
        /// Refresh ComboBox in SINGLE MODE with the person stored by his id in the IdPersonSelected_SINGLEMODE attribute
        /// </summary>
        private void RefreshDisplay_SINGLEMODE()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            if (IdPersonSelected_SINGLEMODE != 0)
            {
                rdcbRapporteurs.Text = personService.FindPerson(IdPersonSelected_SINGLEMODE).PersonSearchTxt;
            }
        }
        #endregion

        #region--- Others methods ---
        /// <summary>
        /// Convert int to string, to convert the value of the search combobox to an id
        /// </summary>
        /// <param name="rdcbValueSelected"></param>
        /// <returns></returns>
        private int ConvertStringToInt(string rdcbValueSelected)
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

        /// <summary>
        /// Launch an alert popup
        /// </summary>
        /// <param name="errorText"></param>
        private void LaunchAlert(string errorText){
            RadWindowAlert.RadAlert(errorText, 400, 150, "Error", "", "images/error.png");
        }
        #endregion

        #endregion
    }
}