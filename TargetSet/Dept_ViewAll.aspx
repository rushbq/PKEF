<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dept_ViewAll.aspx.cs" Inherits="Dept_ViewAll" %>

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
            <%=fn_stringFormat.Filter_Html(Request.QueryString["DeptName"].ToString()) %>
            -
            <%=Param_SetYear %>年度目標</h2>
    </div>
    <div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnDataBound="lvDataList_DataBound"
            OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td width="10%" rowspan="2">
                            月份
                        </td>
                        <td colspan="3">
                            銷售
                        </td>
                        <td colspan="3">
                            接單
                        </td>
                        <td colspan="3">
                            挑戰
                        </td>
                    </tr>
                    <tr class="tdHead">
                        <td width="10%">
                            USD
                        </td>
                        <td width="10%">
                            NTD
                        </td>
                        <td width="10%">
                            RMB
                        </td>
                        <td width="10%">
                            USD
                        </td>
                        <td width="10%">
                            NTD
                        </td>
                        <td width="10%">
                            RMB
                        </td>
                        <td width="10%">
                            USD
                        </td>
                        <td width="10%">
                            NTD
                        </td>
                        <td width="10%">
                            RMB
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    <tr align="center" class="TrGray">
                        <td>
                            總計
                        </td>
                        <%-- 銷售 --%>
                        <td>
                            <asp:Label ID="lb_Total_USD" runat="server" CssClass="styleBlue B"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_Total_NTD" runat="server" CssClass="styleBlue B"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_Total_RMB" runat="server" CssClass="styleBlue B"></asp:Label>
                        </td>
                        <%-- 接單 --%>
                        <td>
                            <asp:Label ID="lb_TotalOrd_USD" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_TotalOrd_NTD" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_TotalOrd_RMB" runat="server"></asp:Label>
                        </td>
                        <%-- 挑戰 --%>
                        <td>
                            <asp:Label ID="lb_TotalCh_USD" runat="server" CssClass="styleReddark B"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_TotalCh_NTD" runat="server" CssClass="styleReddark B"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lb_TotalCh_RMB" runat="server" CssClass="styleReddark B"></asp:Label>
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server" align="center">
                    <td>
                        <%#Eval("SetMonth")%>
                    </td>
                    <%-- 銷售 --%>
                    <td class="styleBlue">
                        <%#fn_stringFormat.C_format(Eval("Amount_USD").ToString())%>
                    </td>
                    <td class="styleBlue">
                        <%#fn_stringFormat.C_format(Eval("Amount_NTD").ToString())%>
                    </td>
                    <td class="styleBlue">
                        <%#fn_stringFormat.C_format(Eval("Amount_RMB").ToString())%>
                    </td>
                    <%-- 接單 --%>
                    <td>
                        <%#fn_stringFormat.C_format(Eval("OrdAmount_USD").ToString())%>
                    </td>
                    <td>
                        <%#fn_stringFormat.C_format(Eval("OrdAmount_NTD").ToString())%>
                    </td>
                    <td>
                        <%#fn_stringFormat.C_format(Eval("OrdAmount_RMB").ToString())%>
                    </td>
                    <%-- 挑戰 --%>
                    <td class="styleReddark" width="150px">
                        <%#fn_stringFormat.C_format(Eval("ChAmount_USD").ToString())%>
                    </td>
                    <td class="styleReddark" width="150px">
                        <%#fn_stringFormat.C_format(Eval("ChAmount_NTD").ToString())%>
                    </td>
                    <td class="styleReddark" width="150px">
                        <%#fn_stringFormat.C_format(Eval("ChAmount_RMB").ToString())%>
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
