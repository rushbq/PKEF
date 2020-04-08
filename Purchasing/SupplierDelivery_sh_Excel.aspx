<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierDelivery_sh_Excel.aspx.cs"
    Inherits="SupplierDelivery_tw_Excel" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <form id="form1" runat="server">
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
        <LayoutTemplate>
            <table border="1">
                <tr>
                    <td width="250px">
                        供應商簡稱
                    </td>
                    <td width="150px">
                        供應商代號
                    </td>
                    <td>
                        一般標準交期
                    </td>
                    <td width="120px">
                        採購人員
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center">
                    <%#Eval("MA002")%>
                </td>
                <td align="center">
                    <%#Eval("MA001")%>
                </td>
                <td align="left">
                    <%#Eval("SupDelDay")%>
                </td>
                <td align="center">
                    <%#Eval("MV002")%>
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div style="padding: 120px 0px 120px 0px; text-align: center">
                查無任何符合資料！
            </div>
        </EmptyDataTemplate>
    </asp:ListView>
    </form>
</body>
</html>
