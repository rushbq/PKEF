<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_TTD_Search.aspx.cs"
    Inherits="Auth_Search" %>

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
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
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
                popupOffset: 10,
                cssAnimations: {
                    enabled: true,
                    show: 'animated flipInX',
                    hide: 'animated flipOutX'
                }
            });
        });
    </script>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $(".TreeView").treeview({
                collapsed: true,
                animated: "fast",
                control: "#sidetreecontrol"
            });

            //全部選取按鈕
            $("#TW_clickAll").click(function () {
                checkAllBox("TW");
            });
            $("#SH_clickAll").click(function () {
                checkAllBox("SH");
            });
            $("#SZ_clickAll").click(function () {
                checkAllBox("SZ");
            });

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
                //排除空值
                if (this.value != '') {
                    if (tmpID != '') {
                        tmpID += ',';
                    }
                    tmpID += this.value;
                }
            });
            //將勾選值填入(分類項目不列入)
            $("#hf_RelID").val(tmpID);

            //觸發click事件
            $('#btn_GetRelID').trigger('click');
        }


        //Function - 全選
        function checkAllBox(objTxt) {
            var MainBox = $("#" + objTxt + "_clickAll");
            var EachBox = $("input[id*='" + objTxt + "_cb_']");

            if (MainBox.attr("checked")) {
                EachBox.each(function () {
                    $(this).attr("checked", true);
                });
            }
            else {
                EachBox.each(function () {
                    $(this).attr("checked", false);
                });
            }

        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>權限管理</a>&gt;<span>日誌查詢權限設定</span>
        </div>
        <div class="h2Head">
            <h2>日誌查詢權限設定 <span class="tipTour" data-smallipop-tour-index="1">&nbsp;<span class="smallipop-hint">*Tips:未設定權限的人，預設就是看自己的資料。</span>
            </span>
            </h2>
        </div>
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="2">設定權限<em class="TableModifyTitleIcon"></em>&nbsp;<input type="button" id="runTour"
                    value="?" title="導覽" class="btnBlock colorDark" />
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 120px">權限授與人
                </td>
                <td class="TableModifyTd">部門：
                <asp:DropDownListGP ID="ddl_Dept" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Dept_SelectedIndexChanged"
                    ValidationGroup="Select" CssClass="tipTour" data-smallipop-tour-index="2" ToolTip="篩選人員名單，非必選">
                </asp:DropDownListGP>
                    &nbsp; 人員：
                <asp:DropDownListGP ID="ddl_Employee" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Employee_SelectedIndexChanged"
                    ValidationGroup="Select" CssClass="tipTour" data-smallipop-tour-index="3" ToolTip="選擇要設定權限的人員，必選">
                </asp:DropDownListGP>
                    &nbsp;
                <asp:Button ID="btn_Search" runat="server" Text="帶出資料" ValidationGroup="Select" CssClass="btnBlock colorGray tipTour"
                    OnClick="btn_Search_Click" data-smallipop-tour-index="4" ToolTip="人員選擇完成後，按下「帶出資料」，開始勾選名單。" />
                    &nbsp;<asp:Label ID="lb_Employee" runat="server" CssClass="styleBluelight"></asp:Label>
                </td>
            </tr>
            <asp:PlaceHolder ID="ph_viewer" runat="server" Visible="false">
                <tr>
                    <td class="TableModifyTdHead" style="width: 120px">選擇要讀取的人員
                    </td>
                    <td class="TableModifyTd">
                        <div id="sidetreecontrol" class="MenuSecControl">
                            <a href="?#">收合</a> | <a href="?#">展開</a>&nbsp;
                        <input type="button" onclick="GetCbxValue()" value="儲存已勾選人員" class="btnBlock colorRed" />
                            <asp:Button ID="btn_GetRelID" runat="server" Text="Button" OnClick="btn_GetRelID_Click"
                                ValidationGroup="Save" Style="display: none" />
                            <asp:HiddenField ID="hf_RelID" runat="server" />
                        </div>
                        <hr class="MenuSecondHr" />
                        <div>
                            <table class="TableS3" width="100%">
                                <tr>
                                    <td class="TS3Head TableS3Dark" style="height: 25px; width: 33%;">
                                        <label>
                                            <input type="checkbox" id="TW_clickAll" /><span class="styleCafe B">台灣</span>
                                        </label>
                                    </td>
                                    <td class="TS3Head TableS3Dark" style="height: 25px; width: 33%;">
                                        <label>
                                            <input type="checkbox" id="SH_clickAll" /><span class="styleChocolate B">上海</span>
                                        </label>
                                    </td>
                                    <td class="TS3Head TableS3Dark" style="height: 25px; width: 33%;">
                                        <label>
                                            <input type="checkbox" id="SZ_clickAll" /><span class="stylePurple B">深圳</span>
                                        </label>
                                    </td>
                                </tr>
                                <tbody>
                                    <tr>
                                        <td valign="top">
                                            <div class="MenuSecond">
                                                <asp:Literal ID="lt_TreeView_TW" runat="server"></asp:Literal>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="MenuSecond">
                                                <asp:Literal ID="lt_TreeView_SH" runat="server"></asp:Literal>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="MenuSecond">
                                                <asp:Literal ID="lt_TreeView_SZ" runat="server"></asp:Literal>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </form>
</body>
</html>
