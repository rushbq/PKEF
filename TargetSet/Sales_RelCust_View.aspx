<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sales_RelCust_View.aspx.cs"
    Inherits="Dept_ViewItem" %>

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
            <%=Param_Display_Name%>
            的關聯客戶</h2>
    </div>
    <div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td width="80px">
                            客戶代號
                        </td>
                        <td width="150px">
                            客戶名稱
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server">
                    <td align="center">
                        <%#Eval("MA001")%>
                    </td>
                    <td align="center">
                        <%#Eval("MA002")%>
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <span class="styleGraylight">-- 未設定關聯客戶 --</span></EmptyDataTemplate>
        </asp:ListView>
    </div>
    <div style="height: 40px">
    </div>
    </form>
</body>
</html>
