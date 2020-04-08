using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Ascx_ScrollIcon : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            if (Session["BackListUrl"] == null)
            {
                Session["BackListUrl"] = "../main.aspx";
            }
            //判斷是否要顯示 - 儲存
            if (ShowSave.Equals("Y"))
            {
                this.pl_Save.Visible = true;
            }
            else
            {
                this.pl_Save.Visible = false;
            }

            //判斷是否要顯示 - 回列表
            if (ShowList.Equals("Y"))
            {
                this.pl_List.Visible = true;
            }
            else
            {
                this.pl_List.Visible = false;
            }

            //判斷是否要顯示 - 回頁首
            if (ShowTop.Equals("Y"))
            {
                this.pl_Top.Visible = true;
            }
            else
            {
                this.pl_Top.Visible = false;
            }

            //判斷是否要顯示 - 至頁尾
            if (ShowBottom.Equals("Y"))
            {
                this.pl_Bottom.Visible = true;
            }
            else
            {
                this.pl_Bottom.Visible = false;
            }
        }
    }

    private string _ShowSave;
    public string ShowSave
    {
        get;
        set;
    }

    private string _ShowList;
    public string ShowList
    {
        get;
        set;
    }

    private string _ShowTop;
    public string ShowTop
    {
        get;
        set;
    }

    private string _ShowBottom;
    public string ShowBottom
    {
        get;
        set;
    }
}