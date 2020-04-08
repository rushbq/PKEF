using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Unauthorized : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            if (Request.QueryString["ErrMsg"] != null) {
                this.lt_ErrMsg.Text = fn_stringFormat.Filter_Html(Request.QueryString["ErrMsg"].ToString());
            }
        }
    }
}