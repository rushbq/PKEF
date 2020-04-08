<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sales_ViewItem.aspx.cs" Inherits="Sales_ViewItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="h2Head">
        <h2>
            [<%=fn_stringFormat.Filter_Html(Request.QueryString["TitleName"].ToString()) %>]
            -
            <%=Param_SetYear %>年<%=fn_stringFormat.Filter_Html(Request.QueryString["SetTitle"].ToString()) %></h2>
    </div>
    <div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnDataBound="lvDataList_DataBound"
            OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td width="80px">
                            月份
                        </td>
                        <td width="150px">
                            金額
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    <tr align="center" class="TrGray">
                        <td>
                            總計
                        </td>
                        <td>
                            <asp:Label ID="lb_Total" runat="server" CssClass="styleChocolate B"></asp:Label>
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server">
                    <td align="center">
                        <%#Eval("SetMonth")%>
                    </td>
                    <td align="center">
                        <%#fn_stringFormat.C_format(Eval("Amount").ToString())%>
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <span class="styleGraylight">-- 未設定目標 --</span></EmptyDataTemplate>
        </asp:ListView>
    </div>
    <div style="height: 40px">
    </div>
    </form>
</body>
</html>
