<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_SetGroup.aspx.cs" Inherits="Auth_SetGroup" %>

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
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
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
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: true,
                animated: "medium",
                control: "#sidetreecontrol"
            });

            //AutoComplete - 群組
            $("#tb_Group_Name").autocomplete('../AC_ADGroups.aspx?Srh_Type=System');
            $("#tb_Group_Name").result(
                function (event, data, formatted) {
                    $("#tb_Group_ID").val(data[1]);
                    $('#btn_Search').trigger('click');
                }
	        );

            /* smallipop - 使用Html */
            $('.tooltip_html').smallipop({
                preferredPosition: 'right',
                theme: 'black',
                popupOffset: 0,
                triggerOnClick: true
            },
             $('#tip1').html()
            );

            //Checkbox階層式全選
            $("input[id*='cb_']").click(function () {
                //取得目前元素的ID
                var thisID = $(this).attr("id");

                //判斷目前元素是否勾選
                if ($(this).attr("checked")) {
                    //檢查每個元素
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", true);  //將其下每一層CheckBox勾選

                        //[取得定義] - 屬性rel (上層編號)
                        var thisLevel = $(this).attr("rel");

                        //[取得定義] - 上層編號
                        var thisUpID = $("#" + thisLevel);

                        //[判斷定義] - 判斷屬性rel是否有定義
                        if (typeof (thisLevel) != "undefined") {
                            //拆解字串
                            var arythisLevel = thisLevel.split("_");
                            for (var i = 0; i < arythisLevel.length - 1; i++) {
                                //判斷上層項目是否勾選
                                if (typeof (thisUpID.attr("checked")) == "undefined") {
                                    thisUpID.attr("checked", true);
                                }
                                //再上層Rel
                                thisLevel = thisUpID.attr("rel");
                                //再上層編號
                                thisUpID = $("#" + thisLevel);
                            }
                        }
                    });
                }
                else {
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", false);
                    });
                }
            });

        });

        //取得Checkbox已勾選的值
        function GetCbxValue() {
            var tmpID = '';
            $("input[id*='cb_']:checkbox:checked").each(function (i) {
                if (tmpID != '')
                    tmpID += ',';
                tmpID += this.value;
            });
            //將勾選值填入
            $("#hf_ProgID").val(tmpID);
            //觸發click事件
            $('#btn_GetProgID').trigger('click');
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>權限管理</a>&gt;<span>權限設定</span>
    </div>
    <div class="h2Head">
        <h2>
            權限設定</h2>
    </div>
    <div class="SysTab">
        <ul>
            <li class="TabAc"><a href="Auth_SetGroup.aspx" style="cursor: pointer;">群組</a></li>
            <li><a href="Auth_SetUser.aspx" style="cursor: pointer;">使用者</a></li>
        </ul>
    </div>
    <!-- Basic Sift -->
    <div class="Sift">
        <ul>
            <li>選擇群組：
                <asp:TextBox ID="tb_Group_Name" runat="server" MaxLength="50" Width="300px" CssClass="tooltip_html"></asp:TextBox>
                <asp:TextBox ID="tb_Group_ID" runat="server" Style="display: none;" MaxLength="40"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_tb_Group_ID" runat="server" ErrorMessage="-&gt; 請選擇正確的「群組」!"
                    ControlToValidate="tb_Group_ID" Display="Dynamic" ForeColor="Red" ValidationGroup="Select"></asp:RequiredFieldValidator>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="帶出資料" ValidationGroup="Select" OnClick="btn_Search_Click"
                    CssClass="btnBlock colorGray" />
                <span class="SiftLight">...填入關鍵字，選擇正確的群組後，按下「帶出資料」</span> </li>
        </ul>
    </div>
    <div id="tip1" style="display: none">
        How to use?<br />
        1. 輸入關鍵字:部門名稱 (ex:專案)<br />
        2. 按下空白鍵
    </div>
    <asp:Panel ID="pl_Data" runat="server" Visible="false">
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">
                    權限表<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt1">
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 100px">
                        群組名稱
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <asp:Label ID="lt_GroupName" runat="server" CssClass="styleBlue B"></asp:Label>
                        <asp:Literal ID="lt_Guid" runat="server" Visible="false"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px">
                        設定停/啟用
                    </td>
                    <td class="TableModifyTd">
                        <asp:Button ID="btn_ChStatus" runat="server" Text="設定停用" OnClick="btn_ChStatus_Click" />
                        <asp:Label ID="lb_UpdTime" runat="server" CssClass="styleRed"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">
                        權限清單
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <div id="sidetreecontrol" class="MenuSecControl">
                            <a href="?#">收合</a> | <a href="?#">展開</a> &nbsp;&nbsp;
                            <input type="button" onclick="GetCbxValue()" value="儲存權限設定" />
                            <asp:Button ID="btn_GetProgID" runat="server" Text="Button" OnClick="btn_GetProgID_Click"
                                ValidationGroup="Save" Style="display: none" />
                            <asp:HiddenField ID="hf_ProgID" runat="server" />
                            &nbsp; <span class="styleRed">(權限判斷會以「個人權限」為優先，因此授與權限時需注意個人權限，<a onclick="location.href='Auth_SearchUser.aspx'"
                                style="cursor: pointer">查詢誰有設定個人權限</a>)</span>
                        </div>
                        <hr class="MenuSecondHr" />
                        <div class="MenuSecond">
                            <asp:Literal ID="lt_AuthProg" runat="server"></asp:Literal>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    function EnterClick(e) {
        // 這一行讓 ie 的判斷方式和 Firefox 一樣。
        if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

        if (e.which == 13) {
            // Submit按鈕
            __doPostBack('btn_Search', '');
            return false;
        }
    }

    document.onkeypress = EnterClick;
</script>
