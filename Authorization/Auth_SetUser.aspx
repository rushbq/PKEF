<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_SetUser.aspx.cs" Inherits="Auth_SetProfile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- catcomplete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/catcomplete/catcomplete.js" type="text/javascript"></script>
    <%-- catcomplete End --%>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
   
    <script type="text/javascript">
        $(function () {
          
            /* Autocomplete - 群組分類(AD使用者) */
            $("#tb_Profile_Name").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_ADUsers.aspx",
                        data: {
                            q: request.term,
                            type: "System"
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category,
                                    Guid: item.Guid
                                }
                            }));
                        }
                    });
                },
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    $("#tb_Profile_ID").val(ui.item.Guid);
                    $('#btn_Search').trigger('click');
                }
            });
        });

        //取得Checkbox已勾選的值
        function GetCbxValue() {
            getCbValue('myTree', 'hf_ProgID');

            //觸發click事件
            $('#btn_GetProgID').trigger('click');
        }
    </script>

    <%-- zTree Start --%>
    <link href="../js/zTree/css/zTreeStyle.css" rel="stylesheet" />
    <script src="../js/zTree/jquery.ztree.core-3.5.min.js"></script>
    <script src="../js/zTree/jquery.ztree.excheck-3.5.min.js"></script>
    <script>
        //zTree 設定
        var setting = {
            view: {
                dblClickExpand: false
            },
            callback: {
                onClick: MMonClick
            },
            check: {
                enable: true
            },
            data: {
                simpleData: {
                    enable: true
                }
            }
        };

        //Event - onClick
        function MMonClick(e, treeId, treeNode) {
            var zTree = $.fn.zTree.getZTreeObj("myTree");
            zTree.expandNode(treeNode);
        }

        //宣告節點
        var zNodes;

        //取得資料
        function getAuthList() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Json_GetAuthList.aspx",
                data: {
                    DataType: 'User',
                    Guid: '<%=Param_Guid %>'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree"), setting, zNodes);

        }

        // 所有節點的收合(true = 展開, false = 折疊)
        function expandAll(objbool) {
            var treeObj = $.fn.zTree.getZTreeObj("myTree");
            treeObj.expandAll(objbool);
        }

        /* 取值(zTree名稱, 要放值的欄位名) */
        function getCbValue(eleName, valName) {

            var treeObj = $.fn.zTree.getZTreeObj(eleName);
            var nodes = treeObj.getCheckedNodes(true);
            var ids = "";
            for (var i = 0; i < nodes.length; i++) {
                //加入分隔符號("||")
                if (ids != "") {
                    ids += "||"
                }
                //取得id值
                ids += nodes[i].id;
            }
            //輸出組合完畢的字串值
            document.getElementById(valName).value = ids;
            return true;
        }

        //Load
        $(document).ready(function () {
            <%if (!string.IsNullOrEmpty(Param_Guid))
              {%>
            getAuthList();
            <%}%>

            //顯示所有節點
            $('#showAll').click(function () {
                expandAll(true);
            });

            //隱藏所有節點
            $('#hideAll').click(function () {
                expandAll(false);
            });

            //複製群組權限按鈕
            $("#copyGP").click(function () {
                //開始複製群組權限
                $.ajax({
                    async: false,
                    cache: false,
                    type: 'POST',
                    dataType: "json",
                    url: "<%=Application["WebUrl"]%>Json_GetAuthList.aspx",
                    data: {
                        DataType: 'COPY_GROUP',
                        Guid: '<%=Param_Guid %>'
                    },
                    error: function () {
                        alert('樹狀選單載入失敗!');
                    },
                    success: function (data) {
                        zNodes = data;

                        //顯示訊息
                        $('#copyGPmessage').show();

                        //載入zTree
                        $.fn.zTree.init($("#myTree"), setting, zNodes);
                    }
                });

            });
        });


    </script>
    <%-- zTree End --%>
    <%-- zTree(群組權限) Start --%>
    <script>
        //宣告節點
        var zNodes_GP;

        //取得資料
        function getAuthList_GP() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Json_GetAuthList.aspx",
                data: {
                    DataType: 'User_IN_Group',
                    Guid: '<%=Param_Guid %>'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes_GP = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree_GP"), setting, zNodes_GP);
        }


        //Load
        $(document).ready(function () {
            <%if (!string.IsNullOrEmpty(Param_Guid))
              {%>
            getAuthList_GP();
            <%}%>

        });

    </script>
    <%-- zTree(群組權限) End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">系統首頁</a>&gt;<a>權限管理</a>&gt;<span>權限設定</span>
        </div>
        <div class="h2Head">
            <h2>權限設定</h2>
        </div>
        <div class="SysTab">
            <ul>
                <li><a href="Auth_SetGroup.aspx" style="cursor: pointer;">群組</a></li>
                <li class="TabAc"><a href="Auth_SetUser.aspx" style="cursor: pointer;">使用者</a></li>
            </ul>
        </div>
        <!-- Basic Sift -->
        <div class="Sift">
            <ul>
                <li>選擇使用者：
                <asp:TextBox ID="tb_Profile_Name" runat="server" MaxLength="50" Width="200px" CssClass="tooltip_html"></asp:TextBox>
                    <asp:TextBox ID="tb_Profile_ID" runat="server" Style="display: none;" MaxLength="40"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfv_tb_Profile_ID" runat="server" ErrorMessage="-&gt; 請選擇正確的「使用者」!"
                        ControlToValidate="tb_Profile_ID" Display="Dynamic" ForeColor="Red" ValidationProfile="Select"></asp:RequiredFieldValidator>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="帶出資料" ValidationProfile="Select"
                        OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <span class="SiftLight">...填入關鍵字，選擇正確的人員後，按下「帶出資料」</span> </li>
            </ul>
        </div>
        <div id="tip1" style="display: none">
            How to use?<br />
            1. 輸入關鍵字:名稱或工號 (ex:10308 or 高)<br />
            2. 按下空白鍵
        </div>
        <asp:Panel ID="pl_Data" runat="server" Visible="false">
            <table class="TableModify">
                <tr class="ModifyHead">
                    <td colspan="4">權限表<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody id="dt1">
                    <tr class="Must">
                        <td class="TableModifyTdHead" style="width: 100px">使用者名稱
                        </td>
                        <td class="TableModifyTd" style="width: 350px">
                            <asp:Label ID="lt_ProfileName" runat="server" CssClass="styleBlue B"></asp:Label>
                            <asp:Literal ID="lt_Guid" runat="server" Visible="false"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 100px">設定停/啟用
                        </td>
                        <td class="TableModifyTd">
                            <asp:Button ID="btn_ChStatus" runat="server" Text="設定停用" OnClick="btn_ChStatus_Click" />
                            <asp:Label ID="lb_UpdTime" runat="server" CssClass="styleRed"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">權限清單
                        </td>
                        <td class="TableModifyTd" colspan="2" valign="top">
                            <div id="sidetreecontrol" class="MenuSecControl">
                                <input type="button" id="showAll" class="btnBlock colorGray" value="展開" />
                                <input type="button" id="hideAll" class="btnBlock colorGray" value="折疊" />
                                <input type="button" onclick="GetCbxValue()" value="設定權限" class="btnBlock colorBlue" />
                                <asp:Button ID="btn_Remove" runat="server" Text="移除權限" OnClick="btn_Remove_Click" CssClass="btnBlock colorRed" />
                                <input type="button" id="copyGP" class="btnBlock colorGreen" value="複製群組權限" />
                            </div>
                            <div id="copyGPmessage" class="alert styleRed" style="display: none;">群組權限已複製，記得按下「設定權限」才會生效!</div>
                            <div style="display: none">
                                <asp:Button ID="btn_GetProgID" runat="server" Text="Button" OnClick="btn_GetProgID_Click"
                                    ValidationProfile="Save" Style="display: none" />
                                <asp:HiddenField ID="hf_ProgID" runat="server" />
                            </div>
                            <hr class="MenuSecondHr" />
                            <div>
                                <ul id="myTree" class="ztree">
                                </ul>
                            </div>
                        </td>
                        <td class="TableModifyTd" valign="top">
                            <div class="MenuSecControl">
                                <span class="styleChocolate B Font14">群組權限參考</span>
                            </div>
                            <hr class="MenuSecondHr" />
                            <div>
                                <ul id="myTree_GP" class="ztree">
                                </ul>
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
