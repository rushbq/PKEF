<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sales_RelCust_Edit.aspx.cs"
    Inherits="Sales_RelCust_Edit" %>

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
    <%-- catcomplete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/catcomplete/catcomplete.js" type="text/javascript"></script>
    <%-- catcomplete End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            /* Autocomplete - 廠商 */
            $("#tb_CustName").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Customer.aspx?f=rel",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: "(" + item.custid + ") " + item.shortName,
                                        value: item.shortName,
                                        custid: item.custid
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    Add_Item(ui.item.label, ui.item.custid, 'ul_ItemList');
                }
            });
        });

    </script>
    <script type="text/javascript">
        //----- 動態欄位 Start -----
        /* 新增項目 
        傳入參數：欄位值, <ul>的編號
        */
        function Add_Item(ObjText, ObjVal, tagUL) {
            if (ObjVal == "") {
                alert('輸入欄空白!');
                return;
            }
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var NewItem = '<li id="li_' + ObjId + '" class="as-selection-item blur">';
            NewItem += ObjText + '<input type="text" class="Item_Val" value="' + ObjVal + '" style="display:none" />';
            NewItem += '<a style="background:transparent" href="javascript:Delete_Item(\'' + ObjId + '\');"><span class="JQ-ui-icon ui-icon-trash"></span></a>';
            NewItem += '</li>';
            $("#" + tagUL).append(NewItem);
        }

        /* 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }
        /* 時間function */
        Date.prototype.Format = function (fmt) { //author: meizz
            var o = {
                "M+": this.getMonth() + 1,                 //月份
                "d+": this.getDate(),                    //日
                "h+": this.getHours(),                   //小時
                "m+": this.getMinutes(),                 //分
                "s+": this.getSeconds(),                 //秒
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度
                "S": this.getMilliseconds()             //毫秒
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }

        /* 取得各項目欄位值
        分隔符號:||||
        傳入參數:接收值的Textbox, <ul>的編號
        */
        function Get_Item(tagVal, tagUL) {
            var Item_Val = $("#" + tagVal);
            Item_Val.val("");
            $("#" + tagUL + " li .Item_Val").each(
                function (i, elm) {
                    var OldCont = Item_Val.val();
                    if (OldCont == '') {
                        Item_Val.val($(elm).val());
                    } else {
                        Item_Val.val(OldCont + '||||' + $(elm).val());
                    }
                }
            );
        }
        //----- 動態欄位 End -----
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <asp:Panel ID="pl_Message" runat="server" CssClass="ListIllusArea BgGray">
        <div class="JQ-ui-state-error">
            <div class="styleEarth">
                <span class="JQ-ui-icon ui-icon-info"></span>Step1. 選擇客戶，系統會自動加入到清單<br />
                <span class="JQ-ui-icon ui-icon-info"></span>Step2. 確認後請按下「存檔」，此次異動的資料才會被儲存</div>
        </div>
    </asp:Panel>
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                客戶關聯設定<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <!-- 資料設定 Start -->
        <tbody>
            <tr class="Must">
                <td class="TableModifyTdHead" style="width: 70px">
                    <em>(*)</em> 人員
                </td>
                <td class="TableModifyTd styleBlue" style="width: 200px">
                    <asp:Literal ID="lt_Display_Name" runat="server"></asp:Literal>
                </td>
                <td class="TableModifyTdHead" style="width: 70px">
                    <em>(*)</em> 客戶
                </td>
                <td class="TableModifyTd" style="width: 250px">
                    <asp:TextBox ID="tb_CustName" runat="server" MaxLength="50" ToolTip="輸入客戶關鍵字" Width="200px"></asp:TextBox>
                    <asp:TextBox ID="tb_Cust_Item_Val" runat="server" Style="display: none" ToolTip="欄位值集合"></asp:TextBox>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    客戶清單
                </td>
                <td class="TableModifyTd" colspan="3" style="min-height:100px">
                    <!-- 動態欄位顯示 -->
                    <div>
                        <ul id="ul_ItemList" class="as-selections">
                            <asp:Literal ID="lt_Items" runat="server"></asp:Literal>
                        </ul>
                    </div>
                </td>
            </tr>
        </tbody>
        <!-- 資料設定 End -->
    </table>
    <div class="SubmitAreaS">
        <asp:Button ID="btn_Save" runat="server" Text="存檔" CssClass="btnBlock colorBlue"
            ValidationGroup="Add" OnClick="btn_Save_Click" OnClientClick="Get_Item('tb_Cust_Item_Val','ul_ItemList');" />
        <input type="button" value="關閉視窗" onclick="parent.$.fancybox.close();" class="btnBlock colorGray" />
        <asp:HiddenField ID="hf_UID" runat="server" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="Add" />
    </div>
    <div style="height: 40px">
    </div>
    </form>
</body>
</html>
