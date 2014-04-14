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
        public List<KeyValuePair<int, bool>> listIdPersonsSelect 
        {
            get
            {
                return listIdPersonsSelect;
            }
            set 
            {
                var personService = ServicesFactory.Resolve<IPersonService>();
                listPersonsSelect = personService.GetByIds(value);
                listIdPersonsSelect = value;
            } 
        }
        public List<View_Persons> listPersonsSelect { get; set; }
        #endregion

        #region events
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void rdGridRapporteurs_PreRender(object sender, System.EventArgs e)
        {
            rdGridRapporteurs.Rebind();
        }

        protected void rdGridRapporteurs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            rdGridRapporteurs.DataSource = listPersonsSelect;
        }
        #endregion

        #region private methods

        #endregion
    }
}