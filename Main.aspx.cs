using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

public partial class Main : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[判斷來源參數]
                if (Request.QueryString["t"] != null)
                {
                    string target = Request.QueryString["t"];
                    string dataID = Request.QueryString["dataID"];

                    switch (target.ToLower())
                    {
                        case "workreport":
                            //工作日誌
                            Response.Redirect("WorkReport/WR_Write.aspx");
                            break;

                        case "ithelp_reply":
                            //資訊需求登記 - 回覆連結
                            Response.Redirect("Recording/IT_HelpEdit.aspx?TraceID={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "ithelp_view":
                            //資訊需求登記 - 觀看連結
                            Response.Redirect("Recording/IT_HelpView.aspx?TraceID={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "ithelp":
                            //資訊需求登記 - 查詢頁
                            Response.Redirect("Recording/IT_HelpSearch.aspx");
                            break;

                        case "ophelp_reply":
                            //品號需求登記 - 回覆連結
                            Response.Redirect("myOPHelp/OP_HelpEdit.aspx?TraceID={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "ophelp_view":
                            //品號需求登記 - 觀看連結
                            Response.Redirect("myOPHelp/OP_HelpView.aspx?TraceID={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "ophelp":
                            //品號需求登記 - 查詢頁
                            Response.Redirect("myOPHelp/OP_HelpSearch.aspx");
                            break;

                        case "inquiry":
                            //官網inquiry - 觀看連結
                            Response.Redirect("myMarket/Msg_Search.aspx?Keyword={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "sc-inquiry":
                            //玩具網站inquiry - 觀看連結
                            Response.Redirect("myMarket/ToyMsg_Search.aspx?Keyword={0}".FormatThis(Server.UrlEncode(dataID)));
                            break;

                        case "dwfiles":
                            //官網下載
                            Response.Redirect("myDownload/myDW.aspx");
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}