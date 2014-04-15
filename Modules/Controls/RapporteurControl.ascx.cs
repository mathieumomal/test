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

        #endregion

        #region public properties
        public Boolean isPrimarySelectable { get; set; }
        public Boolean isMultiple { get; set; }
        public List<KeyValuePair<int, bool>> listIdPersonsSelect { get; set; }
        public List<View_Persons> listPersonsSelect { get; set; }
        #endregion

        #region events
        protected void Page_Load(object sender, EventArgs e)
        {
            var exampleListId = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(14, false),
                    new KeyValuePair<int, bool>(23, false),
                    new KeyValuePair<int, bool>(25, false),
                    new KeyValuePair<int, bool>(10, false),
                    new KeyValuePair<int, bool>(14, false),
                    new KeyValuePair<int, bool>(6, false)
                };
            listIdPersonsSelect = exampleListId;
            var personService = ServicesFactory.Resolve<IPersonService>();
            listPersonsSelect = personService.GetByIds(listIdPersonsSelect);
        }

        //Table
        protected void rdGridRapporteurs_PreRender(object sender, System.EventArgs e)
        {
            rdGridRapporteurs.Rebind();
        }

        protected void rdGridRapporteurs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            rdGridRapporteurs.DataSource = listPersonsSelect;
        }

        //Search
        protected void rdcbRapporteurs_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                var svc = ServicesFactory.Resolve<IPersonService>();
                var personsFound = svc.LookFor(e.Text);
                BindDropDownData(personsFound);
            }
        }
        #endregion

        #region private methods
        private void BindDropDownData(List<DomainClasses.View_Persons> personsList)
        {
            rdcbRapporteurs.DataSource = personsList;
            rdcbRapporteurs.DataTextField = "PersonSearchTxt";
            rdcbRapporteurs.DataValueField = "Email";
            rdcbRapporteurs.DataBind();
        }
        #endregion
    }
}