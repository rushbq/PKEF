<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoSales_Corp_Edit.aspx.cs" Inherits="InfoSales_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <script>
        //判斷核取方塊選項(ClientValidate)
        function check_CbItems(sender, args) {
            var flagNum = 0;
            var optList = document.getElementById("cbl_Corp");
            var inArr = optList.getElementsByTagName('input');
            for (var i = 0; i < inArr.length; i++) {
                if (inArr[i].type == "checkbox") {
                    if (inArr[i].checked == true) {
                        flagNum += 1;
                    }
                }
            }
            if (flagNum == 0) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">資料維護<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <!-- 資料設定 Start -->
            <tbody>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 120px">人員
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_UserID" runat="server" CssClass="styleBluelight"></asp:Label>
                        , 
                        <asp:Label ID="lb_UserName" runat="server" CssClass="styleRed B"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">設定公司別
                    </td>
                    <td class="TableModifyTd">
                        <asp:CheckBoxList ID="cbl_Corp" runat="server"></asp:CheckBoxList>
                        <asp:CustomValidator ID="cv_check_CbItems" runat="server" ErrorMessage="請選擇「公司別」" Display="Dynamic"
                            ClientValidationFunction="check_CbItems" ValidationGroup="Add" CssClass="styleRed help-block"></asp:CustomValidator>
                    </td>
                </tr>
            </tbody>
            <!-- 資料設定 End -->
        </table>
        <div class="SubmitAreaS">
            <asp:Button ID="btn_Save" runat="server" Text="存檔" Width="90px" CssClass="btnBlock colorBlue"
                ValidationGroup="Add" OnClick="btn_Save_Click" />
            <input type="button" value="關閉視窗" onclick="parent.$.fancybox.close();" style="width: 90px"
                class="btnBlock colorGray" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                ShowMessageBox="true" ValidationGroup="Add" />
        </div>
        <div style="height: 40px">
        </div>
    </form>
</body>
</html>
