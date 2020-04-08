<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Write_Edit.aspx.cs" Inherits="WR_Write_Edit" ValidateRequest="false" %>

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
    <%-- smallipop Start --%>
    <link href="../js/smallipop/css/contrib/animate.min.css" rel="stylesheet" type="text/css" />
    <link href="../js/smallipop/css/jquery.smallipop.min.css" rel="stylesheet" type="text/css" />
    <script src="../js/smallipop/jquery.smallipop.min.js" type="text/javascript"></script>
    <script src="../js/smallipop/modernizr.min.js" type="text/javascript"></script>
    <%-- smallipop End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            /* smallipop, tour效果 */
            $('#runTour').click(function () {
                $('.tipTour').smallipop('tour');
            });
            $('.tipTour').smallipop({
                theme: 'black',
                popupOffset: 5,
                cssAnimations: {
                    enabled: true,
                    show: 'animated flipInX',
                    hide: 'animated flipOutX'
                }
            });

            //變更類別選單時，清除 常用類別
            $('select#ddl_Class').change(function () {
                var idx = $(this).get(0).selectedIndex;
                if (idx > 0) {
                    //清除已勾選
                    var cb_Items = $('input[id*="rbl_HotClass"]');
                    for (var i = 0; i < cb_Items.length; i++) {
                        cb_Items[i].checked = false;
                    }
                }
            });
            //選擇常用類別時，清除 類別選單
            $('input[id*="rbl_HotClass"]').click(function () {
                //清除類別選單已選取
                $("select#ddl_Class")[0].selectedIndex = 0;
            });

            /* 日期選擇器 */
            $("#tb_TaskDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                minDate: 0
            });

        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">
                    <asp:Literal ID="lt_EditType" runat="server">新增</asp:Literal>工作項目<em class="TableModifyTitleIcon"></em>&nbsp;&nbsp;
                <input type="button" id="runTour" value="?" title="導覽" class="btnBlock colorDark" />
                </td>
            </tr>
            <!-- 資料設定 Start -->
            <tbody>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 80px">系統編號
                    </td>
                    <td class="TableModifyTd styleBlue">
                        <asp:Label ID="lb_Task_ID" runat="server" Text="系統自動編號" Font-Bold="False" ForeColor="Blue"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 日期
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_TaskDate" runat="server" Style="text-align: center" Width="80px" CssClass="tipTour" data-smallipop-tour-index="1" ToolTip="可填入今日及未來的日期"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_TaskDate" runat="server" ErrorMessage="-&gt; 請填入「日期」!"
                            ControlToValidate="tb_TaskDate" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rev_tb_TaskDate" runat="server" ErrorMessage="-&gt; 「日期」格式錯誤!"
                            ControlToValidate="tb_TaskDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                            Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RegularExpressionValidator>
                        <asp:CompareValidator ID="cv_Date" runat="server" ErrorMessage="-&gt;「日期」不能是過去的日期!"
                            Type="Date" Operator="LessThanEqual" ControlToValidate="tb_DateNow" ControlToCompare="tb_TaskDate"
                            Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                        <asp:TextBox ID="tb_DateNow" runat="server" Style="display: none"></asp:TextBox>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 類別
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 5px">
                            <asp:DropDownList ID="ddl_Class" runat="server" CssClass="tipTour" data-smallipop-tour-index="2"
                                ToolTip="依工作內容選擇適合的類別">
                            </asp:DropDownList>
                        </div>
                        <asp:Panel ID="pl_Hot" runat="server" CssClass="styleGraylight">
                            常用類別：
                        <asp:RadioButtonList ID="rbl_HotClass" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
                            CssClass="styleCafe">
                        </asp:RadioButtonList>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em>工作項目
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_TaskName" runat="server" MaxLength="60" Width="95%" CssClass="tipTour"
                            data-smallipop-tour-index="3" ToolTip="填寫工作項目的簡述 (必填)，最多 30 個中文字。"></asp:TextBox><br />
                        <asp:RequiredFieldValidator ID="rfv_tb_TaskName" runat="server" ErrorMessage="-&gt; 請輸入「工作項目」!"
                            ControlToValidate="tb_TaskName" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        <span class="SiftLight">(字數上限：30 個中文字)</span>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">內容說明
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_Remark" runat="server" TextMode="MultiLine" Width="95%" Height="200px"
                            CssClass="tipTour" data-smallipop-tour-index="4" ToolTip="填寫工作項目的詳細說明 (可空白)，最多 5000 個字。"></asp:TextBox>
                        <br />
                        <span class="SiftLight">(字數上限：5000 個中文字)</span>
                    </td>
                </tr>
            </tbody>
            <!-- 資料設定 End -->
        </table>
        <div class="SubmitAreaS">
            <asp:Button ID="btn_Save" runat="server" Text="存檔" Width="90px" CssClass="btnBlock colorBlue"
                ValidationGroup="Add" OnClick="btn_Save_Click" />
            <input type="button" value="關閉視窗" onclick="parent.window.hs.getExpander().close();"
                style="width: 90px" class="btnBlock colorGray" />
            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
            <asp:HiddenField ID="hf_UID" runat="server" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                ShowMessageBox="true" ValidationGroup="Add" />
        </div>
        <div style="height: 20px">
        </div>
    </form>
</body>
</html>
