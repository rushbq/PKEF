<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoSales_View.aspx.cs" Inherits="InfoSales_View" %>

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
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">資料明細<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <!-- 資料設定 Start -->
            <tbody>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 100px">人員
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_UserID" runat="server" CssClass="styleBluelight"></asp:Label>
                        , 
                        <asp:Label ID="lb_UserName" runat="server" CssClass="styleRed B"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 100px">E-Mail
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Email" runat="server" CssClass="styleCafe"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">NickName
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_NickName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">ERP登入代號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_ERP_LoginID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">ERP員工代號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_ERP_UserID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">電話
                    </td>
                    <td class="TableModifyTd">

                        <asp:Label ID="lb_Tel" runat="server"></asp:Label>&nbsp;
                        分機:
                        <asp:Label ID="lb_TelExt" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">行動電話
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Mobile" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">Skype
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_IM_Skype" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="TableModifyTdHead">Line
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_IM_Line" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">QQ
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_IM_QQ" runat="server"></asp:Label>
                    </td>
                </tr>
            </tbody>
            <!-- 資料設定 End -->
        </table>
        <div class="SubmitAreaS">
            <input type="button" value="關閉視窗" onclick="parent.$.fancybox.close();" style="width: 90px"
                class="btnBlock colorGray" />
        </div>
        <div style="height: 40px">
        </div>
    </form>
</body>
</html>
