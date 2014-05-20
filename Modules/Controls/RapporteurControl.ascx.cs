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
    ///--- About this control ---
    /// This control provide two main mode to select person(s) :
    ///- Edit MODE
    ///- View MODE
    ///To choose it we need to use the 'IsEditMode' attribute
    ///
    ///Moreover you can choose : 
    ///- A 'choice' mode (One person (SINGLEMODE) or more than one (MULTIMODE)) defined by 'IsSinglePersonMode' attribute
    ///- A multiple select mode (IN CASE OF CHOICE MODE => MULTIMODE ; we can choose to select one 
    ///('SelectableMode' = CONST_RAPPORTEURS_SELECTABLEMODE.single) 
    ///or more than one person 
    ///('SelectableMode' = CONST_RAPPORTEURS_SELECTABLEMODE.multi) )
    ///OR no one 
    ///('SelectableMode' = CONST_RAPPORTEURS_SELECTABLEMODE.none) )
    ///
    ///To initialize this control with person(s) : 
    ///- IN SINGLEMODE : set 'IdPersonSelected_SINGLEMODE' by an id
    ///- IN MULTIMODE : set 'ListIdPersonsSelected_MULTIMODE' by a list of ids
    ///
    ///To get results : 
    ///we can use this last same attributes
    public partial class RapporteurControl : System.Web.UI.UserControl
    {
        #region constants
        //ViewStates
        private const string RAPPORTEURS_VIEWSTATE_DATASOURCE = "RapporteursDataSource";
        private const string RAPPORTEURS_VIEWSTATE_ID = "RapporteursIds";
        private const string RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE = "IsSinglePersonMode";
        private const string RAPPORTEURS_VIEWSTATE_ISEDITMODE = "IsEditMode";
        private const string RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE = "IdPersonSelected_SINGLEMODE";
        private const string RAPPORTEURS_VIEWSTATE_SELECTABLEMODE = "SelectableMode";
        private const string RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED = "ListIdPersonSelect";
        private const string RAPPORTEURS_VIEWSTATE_BASEADDRESS = "RapporteurLinkBaseAddress";
        private const string RAPPORTEURS_VIEWSTATE_COLUMNNAME = "SelectableColumnName";

        //Columns
        private const string RAPPORTEURS_COLUMN_SELECTABLE = "selectable";
        private const string RAPPORTEURS_COLUMN_DELETE = "delete";
        private const string RAPPORTEURS_COLUMN_EMAIL = "email";
        private const string RAPPORTEURS_COLUMN_NAME = "name";
        private const string RAPPORTEURS_COLUMN_NAMEHYPERLINK = "nameHyperLink";

        //CssClass
        private const string RAPPORTEURS_CSSCLASS_EDITMODE_ONEPERSONMODE = "editModeOnePersonMode";
        private const string RAPPORTEURS_CSSCLASS_VIEWMODE_ONEPERSONMODE = "viewModeOnePersonMode";

        private const string RAPPORTEURS_CSSCLASS_EDITMODE_MULTIPERSONMODE = "editModeMultiPersonMode";
        private const string RAPPORTEURS_CSSCLASS_VIEWMODE_MULTIPERSONMODE = "viewModeMultiPersonMode";

        public enum RapporteursSelectablemode
        {
            Multi,
            Single,
            None
        }
        #endregion

        #region public properties

        /// <summary>
        /// Provide the edit current mode ; EDIT MODE (true) or VIEW MODE (false)
        /// </summary>
        public bool IsEditMode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISEDITMODE] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISEDITMODE] = false;

                return (bool)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISEDITMODE];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISEDITMODE] = value;
            }
        }

        /// <summary>
        /// One person could be selected (isOneMode = true) or multiple persons could be selected (isOneMode = false)
        /// </summary>
        public bool IsSinglePersonMode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] = false;

                return (bool)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ISSINGLEPERSONMODE] = value;
            }
        }

        /// <summary>
        /// This module provides a selection system
        /// </summary>
        public RapporteursSelectablemode SelectableMode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] = RapporteursSelectablemode.None;

                return (RapporteursSelectablemode)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_SELECTABLEMODE];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_SELECTABLEMODE] = value;
            }
        }

        /// <summary>
        /// List of selected persons
        /// </summary>
        public List<int> ListIdPersonSelect
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] = new List<int>();

                return (List<int>)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_LISTIDPERSONSELECTED] = value;
            }
        }

        /// <summary>
        /// IN SELECTABLEMODE (SINGLE) set header title
        /// </summary>
        public string SelectableColumnName
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_COLUMNNAME] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_COLUMNNAME] = "Primary";

                return (String)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_COLUMNNAME];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_COLUMNNAME] = value;
            }
        }

        /// <summary>
        /// For the Single selected mode, this attribute provides the last person's id choosen
        /// </summary>
        public int IdPersonSelected_singlemode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] = 0;

                return (int)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_IDPERSONSELECTEDSINGLEMODE] = value;
            }
        }

        /// <summary>
        /// List of choosen person's id
        /// </summary>
        public List<int> ListIdPersonsSelected_multimode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ID] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ID] = new List<int>();

                return (List<int>)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ID];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_ID] = value;
            }
        }

        /// <summary>
        /// Datasource for the radgrid which contains the list of persons (MODELS)
        /// </summary>
        public List<View_Persons> DataSource_multimode
        {
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_DATASOURCE] == null)
                    ViewState[ClientID + RAPPORTEURS_VIEWSTATE_DATASOURCE] = new List<View_Persons>();

                return (List<View_Persons>)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_DATASOURCE];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_DATASOURCE] = value;
            }
        }

        /// <summary>
        /// Enables to set the address of the external link to the person, if a link should be displayed.
        /// </summary>
        public string PersonLinkBaseAddress { 
            get
            {
                if (ViewState[ClientID + RAPPORTEURS_VIEWSTATE_BASEADDRESS] == null)
                    return "";

                return (string)ViewState[ClientID + RAPPORTEURS_VIEWSTATE_BASEADDRESS];
            }
            set
            {
                ViewState[ClientID + RAPPORTEURS_VIEWSTATE_BASEADDRESS] = value;
            }
        }
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitControl();
            }
        }

        /// <summary>
        /// Initialization of control
        /// </summary>
        private void InitControl()
        {
            var selectableColumn = (GridClientSelectColumn)rdGridRapporteurs.MasterTableView.GetColumn(RAPPORTEURS_COLUMN_SELECTABLE);
            var deletedColumn = (GridButtonColumn)rdGridRapporteurs.MasterTableView.GetColumn(RAPPORTEURS_COLUMN_DELETE);
            var emailColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(RAPPORTEURS_COLUMN_EMAIL);
            var nameColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(RAPPORTEURS_COLUMN_NAME);
            var nameHyperLinkColumn = (GridTemplateColumn)rdGridRapporteurs.MasterTableView.GetColumn(RAPPORTEURS_COLUMN_NAMEHYPERLINK);
            if (IsEditMode)//EDIT MODE CONFIG
            {
                if (SelectableMode.Equals(RapporteursSelectablemode.Single))
                {
                    selectableColumn.Visible = true;
                    rdGridRapporteurs.AllowMultiRowSelection = false;
                    rdGridRapporteurs.ClientSettings.EnablePostBackOnRowClick = true;
                    selectableColumn.HeaderText = SelectableColumnName;
                }
                else if (SelectableMode.Equals(RapporteursSelectablemode.Multi))
                {
                    selectableColumn.Visible = true;
                    rdGridRapporteurs.AllowMultiRowSelection = true;
                    rdGridRapporteurs.ClientSettings.EnablePostBackOnRowClick = true;
                }

                if (!IsSinglePersonMode)
                {
                    panelRapporteurControl.CssClass = RAPPORTEURS_CSSCLASS_EDITMODE_MULTIPERSONMODE;
                    RefreshDataSourceFromListIds_MULTIMODE();
                    RefreshDisplay_MULTIMODE();
                }
                else
                {
                    panelRapporteurControl.CssClass = RAPPORTEURS_CSSCLASS_EDITMODE_ONEPERSONMODE;
                    rdGridRapporteurs.Visible = false;
                    RefreshDisplay_SINGLEMODE();
                }
            }
            else//VIEW MODE CONFIG
            {
                if (!IsSinglePersonMode)//MULTI PERSON MODE
                {
                    panelRapporteurControl.CssClass = RAPPORTEURS_CSSCLASS_VIEWMODE_MULTIPERSONMODE;

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
                    panelRapporteurControl.CssClass = RAPPORTEURS_CSSCLASS_VIEWMODE_ONEPERSONMODE;

                    rdGridRapporteurs.Visible = false;

                    rdcbRapporteurs.Enabled = false;

                    btnAddRapporteur.Visible = false;
                    lblAddRapporteur.Visible = false;
                    RefreshDisplay_SINGLEMODE();
                }
            }
        }

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
                if (!ListIdPersonsSelected_multimode.Contains(personIdToAdd))
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
            foreach (GridDataItem item in rdGridRapporteurs.MasterTableView.Items)
            {
                if (item.Selected)
                {
                    var ID = ConvertStringToInt(item.OwnerTableView.DataKeyValues[item.ItemIndex]["PERSON_ID"].ToString());
                    temp.Add(ID);
                }
            }
            ListIdPersonSelect = temp;
        }

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
                IdPersonSelected_singlemode = ConvertStringToInt(e.Value);
            }
        }

        #endregion

        #region private methods

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

        /// <summary>
        /// Refresh the Datasource attribute thanks to the list of persons' ids (MULTIMODE)
        /// </summary>
        private void RefreshDataSourceFromListIds_MULTIMODE()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            DataSource_multimode = personService.GetByIds(ListIdPersonsSelected_multimode);
        }

        /// <summary>
        /// Refresh the Datasource by the last person added
        /// </summary>
        private void AddPerson_MULTIMODE(int id)
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            ListIdPersonsSelected_multimode.Add(id);
            DataSource_multimode.Add(personService.FindPerson(id));
        }

        /// <summary>
        /// Remove someone of the list
        /// </summary>
        /// <param name="id"></param>
        private void RemovePerson_MULTIMODE(int id)
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            ListIdPersonsSelected_multimode.Remove(id);
            DataSource_multimode.Remove(DataSource_multimode.SingleOrDefault(s => s.PERSON_ID == id));
        }

        /// <summary>
        /// Refresh radgrid (rebind)
        /// </summary>
        private void RefreshDisplay_MULTIMODE()
        {
            if (!IsEditMode)
            {
                if (ListIdPersonSelect.Count() != 0)
                {
                    foreach (View_Persons person in DataSource_multimode)
                    {
                        if (ListIdPersonSelect.Contains(person.PERSON_ID))
                        {
                            person.FIRSTNAME = new StringBuilder()
                                .Append("<strong>")
                                .Append(person.FIRSTNAME)
                                .Append("</strong>")
                                .ToString();
                            person.LASTNAME = new StringBuilder()
                                .Append("<strong>")
                                .Append(person.LASTNAME)
                                .Append("</strong>")
                                .ToString();
                        }
                    }
                }
            }
            rdGridRapporteurs.DataSource = DataSource_multimode;
            rdGridRapporteurs.Rebind();
            if (IsEditMode)
            {
                if (ListIdPersonSelect.Count() != 0 && SelectableMode != RapporteursSelectablemode.None)
                {
                    foreach (GridDataItem item in rdGridRapporteurs.MasterTableView.Items)
                    {
                        var ID = ConvertStringToInt(item.OwnerTableView.DataKeyValues[item.ItemIndex]["PERSON_ID"].ToString());
                        if (ListIdPersonSelect.Contains(ID))
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refresh ComboBox in SINGLE MODE with the person stored by his id in the IdPersonSelected_SINGLEMODE attribute
        /// </summary>
        private void RefreshDisplay_SINGLEMODE()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            if (IdPersonSelected_singlemode != 0)
            {
                rdcbRapporteurs.Text = personService.FindPerson(IdPersonSelected_singlemode).PersonSearchTxt;
            }
        }

        /// <summary>
        /// Convert int to string, to convert the value of the search combobox to an id
        /// </summary>
        /// <param name="rdcbValueSelected"></param>
        /// <returns></returns>
        private int ConvertStringToInt(string rdcbValueSelected)
        {
            int personID;
            if (Int32.TryParse(rdcbValueSelected, out personID))
                return personID;
            return 0;
        }

        /// <summary>
        /// Launch an alert popup
        /// </summary>
        /// <param name="errorText"></param>
        private void LaunchAlert(string errorText)
        {
            RadWindowAlert.RadAlert(errorText, 400, 150, "Error", "", "images/error.png");
        }

        #endregion
    }
}