using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Controls
{
    public partial class FakeControl : System.Web.UI.UserControl
    {
        public int BaseSelectedColor { get; set; }
        protected int currentColorIndex;
        protected String[] colors = { "Red", "Blue", "Green", "Yellow" };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                currentColorIndex =
                    Int16.Parse(ViewState["currentColorIndex"].ToString());
            }
            else
            {
                currentColorIndex = BaseSelectedColor;
                DisplayColor();
            }
        }

        protected void DisplayColor()
        {
            textColor.Text = colors[currentColorIndex];
            ViewState["currentColorIndex"] = currentColorIndex.ToString();
        }

        protected void buttonUp_Click(object sender, EventArgs e)
        {
            if (currentColorIndex == 0)
            {
                currentColorIndex = colors.Length - 1;
            }
            else
            {
                currentColorIndex -= 1;
            }
            DisplayColor();
        }

        protected void buttonDown_Click(object sender, EventArgs e)
        {
            if (currentColorIndex == (colors.Length - 1))
            {
                currentColorIndex = 0;
            }
            else
            {
                currentColorIndex += 1;
            }
            DisplayColor();
        }
    }
}