<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cust_Edit.aspx.cs" Inherits="Cust_Edit" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />

    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.1/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>

    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.11/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/dataTables.bootstrap.min.js"></script>
    <script>
        $(function () {
            //使用DataTable
            $('#listTable').DataTable({
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false       //筆數資訊
            });
        });
    </script>
    <%-- DataTable End --%>

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
        function getItemList() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Ajax_Data/Json_ReportList.aspx",
                data: { DataType: "Dealer" },
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
                //只取開頭為'v_'的值
                var myval = nodes[i].id;

                if (myval.substring(0, 2) == "v_") {
                    //字串組合, 加入分隔符號("||")
                    if (ids != "") {
                        ids += "||"
                    }

                    //取得id值
                    ids += myval.replace("v_", "");
                }
            }
            //輸出組合完畢的字串值
            document.getElementById(valName).value = ids;
            return true;
        }

        //Load
        $(document).ready(function () {
            getItemList();

            //顯示所有節點
            $('#showAll').click(function () {
                expandAll(true);
            });

            //隱藏所有節點
            $('#hideAll').click(function () {
                expandAll(false);
            });

        });


    </script>
    <%-- zTree End --%>

    <script>
        $(function () {
            //Click事件, 觸發儲存
            $("#triggerDBSave").click(function () {
                $('#btn_DBSave').trigger('click');
            });


            //smooth scroll
            $("#mySideMenu a").on('click', function (event) {
                // Make sure this.hash has a value before overriding default behavior
                if (this.hash !== "") {
                    // Prevent default anchor click behavior
                    event.preventDefault();

                    // Store hash
                    var hash = this.hash;

                    // Using jQuery's animate() method to add smooth page scroll
                    // The optional number (800) specifies the number of milliseconds it takes to scroll to the specified area
                    $('html, body').animate({
                        scrollTop: $(hash).offset().top
                    }, 300, function () {

                        // Add hash (#) to URL when done scrolling (default click behavior)
                        window.location.hash = hash;
                    });
                }  // End if
            });
        });

    </script>

</head>
<body class="MainArea" data-spy="scroll" data-target="#mySideMenu" data-offset="15">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>基本資料維護</a>&gt;<span>客戶基本資料</span>
        </div>
        <div class="h2Head">
            <h2>客戶基本資料</h2>
        </div>
        <div class="row">
            <div class="col-sm-10">
                <!-- 基本資料 -->
                <div id="baseData" class="table-responsive">
                    <table class="TableModify table table-bordered">
                        <tr class="ModifyHead">
                            <td colspan="4"><i class="fa fa-file" aria-hidden="true"></i>&nbsp;基本資料<em class="TableModifyTitleIcon"></em></td>
                        </tr>
                        <tbody>
                            <tr>
                                <td class="TableModifyTdHead" style="width: 10%">客戶代號
                                </td>
                                <td class="TableModifyTd styleBlue B" style="width: 40%">
                                    <asp:Literal ID="lt_CustID" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead" style="width: 10%">主要資料庫
                                </td>
                                <td class="TableModifyTd styleRed B" style="width: 40%">
                                    <asp:Literal ID="lt_DBName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">客戶簡稱
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_CustSortName" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">出貨庫別
                                </td>
                                <td class="TableModifyTd styleEarth B">
                                    <asp:Literal ID="lt_SWName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">客戶全稱
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_CustFullName" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">地區/國家
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Label ID="lb_AreaName" runat="server" CssClass="label label-default"></asp:Label>
                                    <asp:Label ID="lb_CountryName" runat="server" CssClass="label label-default"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">客戶Email
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_Email" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">交易幣別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_Currency" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">出貨地址
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_ShipAddr" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">負責業務
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_RepSales" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <!-- 資料庫設定 -->
                <div id="dbData" class="table-responsive">
                    <table class="TableModify table table-bordered">
                        <tr class="ModifyHead">
                            <td colspan="4"><i class="fa fa-database" aria-hidden="true"></i>&nbsp;資料庫設定<em class="TableModifyTitleIcon"></em></td>
                        </tr>
                        <tbody>
                            <tr>
                                <td class="TableModifyTdHead" style="width: 10%">主要資料庫
                                </td>
                                <td class="TableModifyTd" style="width: 40%">
                                    <asp:DropDownList ID="ddl_MainDB" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td class="TableModifyTdHead" style="width: 10%">出貨庫別
                                </td>
                                <td class="TableModifyTd" style="width: 40%">
                                    <asp:DropDownList ID="ddl_SWID" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddl_SWID" runat="server" ErrorMessage="請選擇「出貨庫別」" ControlToValidate="ddl_SWID" Display="Dynamic" ValidationGroup="Add" ForeColor="Red" CssClass="help-block"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">報價資料庫
                                </td>
                                <td class="TableModifyTd" colspan="3">
                                    <asp:CheckBoxList ID="cbl_PriceDB" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="table table-bordered"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">發票類型
                                </td>
                                <td class="TableModifyTd">
                                    <asp:DropDownList ID="ddl_InvType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">-- 發票類型 --</asp:ListItem>
                                        <asp:ListItem Value="0">專票</asp:ListItem>
                                        <asp:ListItem Value="2">普票</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="TableModifyTdHead">
                                </td>
                                <td class="TableModifyTd">
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">維護資訊
                                </td>
                                <td class="TableModifyTd" colspan="3">
                                    <div class="row">
                                        <div class="col-xs-6">
                                            最後更新者：<asp:Literal ID="lt_Update_Who" runat="server" Text="資料維護中"></asp:Literal>
                                        </div>
                                        <div class="col-xs-6">
                                            最後更新時間：<asp:Literal ID="lt_Update_Time" runat="server" Text="資料維護中"></asp:Literal>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td class="SubmitAreaS text-right" colspan="4">
                                    <input type="button" id="triggerDBSave" class="btn btn-primary btn-sm" value="儲存設定" />
                                    <asp:Button ID="btn_DBSave" runat="server" Text="存檔trigger" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />

                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                                        ShowMessageBox="true" ValidationGroup="Add" />
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                </div>

                <!-- 報表設定 -->
                <div id="rptData" class="table-responsive">
                    <table class="TableModify table table-bordered">
                        <tr class="ModifyHead">
                            <td><i class="fa fa-bar-chart" aria-hidden="true"></i>&nbsp;報表設定<em class="TableModifyTitleIcon"></em></td>
                        </tr>
                        <tbody>
                            <tr>
                                <td class="TableModifyTd bg-warning">
                                    <div>
                                        <input type="button" id="showAll" class="btn btn-default" value="展開" />
                                        <input type="button" id="hideAll" class="btn btn-default" value="折疊" />
                                        <asp:LinkButton ID="lbtn_AddItem" runat="server" CssClass="btn btn-primary" ValidationGroup="AddItem" OnClientClick="getCbValue('myTree', 'tb_Items');blockBox1('AddItem','項目加入中...');" Text="加入報表關聯" OnClick="lbtn_AddItem_Click"></asp:LinkButton>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Items" runat="server" ErrorMessage="未勾選報表" ControlToValidate="tb_Items" Display="Dynamic" ValidationGroup="AddItem" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="help-block">(加入此次勾選的項目後，前次已加入的項目會清除)</div>
                                    <ul id="myTree" class="ztree">
                                    </ul>
                                    <asp:TextBox ID="tb_Items" runat="server" Style="display: none"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTd">
                                    <div class="panel-body collapse in table-responsive">
                                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                                            <LayoutTemplate>
                                                <table id="listTable" class="table table-bordered table-striped">
                                                    <thead>
                                                        <tr>
                                                            <th>報表編號</th>
                                                            <th>報表描述</th>
                                                            <th style="width: 10%">&nbsp;</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                    </tbody>
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%#Eval("ID") %>
                                                    </td>
                                                    <td>
                                                        <%#Eval("Label") %>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除?')" ToolTip="刪除">
                       <i class="fa fa-trash-o fa-lg"></i>
                                                        </asp:LinkButton>

                                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("ID") %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <div class="text-center text-danger" style="padding: 10px 0px 10px 0px;">
                                                    <h3><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;尚未設定報表關聯</h3>
                                                </div>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <!-- 其他資料 -->
                <div id="otherData" class="table-responsive">
                    <table class="TableModify table table-bordered">
                        <tbody>
                            <tr class="ModifyHead">
                                <td colspan="4"><i class="fa fa-file-text" aria-hidden="true"></i>&nbsp;其他資料<em class="TableModifyTitleIcon"></em>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead" style="width: 10%">負責人
                                </td>
                                <td class="TableModifyTd" style="width: 40%">
                                    <asp:Literal ID="lt_MA004" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead" style="width: 10%">連絡人
                                </td>
                                <td class="TableModifyTd" style="width: 40%">
                                    <asp:Literal ID="lt_MA005" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead styleRed">戶名(開票用)<br />(限100字,含空白)<br />客戶英文全名
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA110" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead styleRed">稅號(開票用)<br />(限20字,含空白)<br />銀行帳號(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA071" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">TEL_NO(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA006" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">TEL_NO(二)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA007" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">FAX_NO
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA008" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead"></td>
                                <td class="TableModifyTd"></td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">通路別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA017" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">最近交易日
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA022" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">登記地址(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA023" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">登記地址(二)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA024" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">發票地址(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA025" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">發票地址(二)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA026" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">送貨地址(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA027" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead"></td>
                                <td class="TableModifyTd"></td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">價格條件
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA030" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">付款條件
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA031" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">發票聯數
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA037" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">課稅別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA038" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">郵遞區號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA040" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">收款方式
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA041" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">運輸方式
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA048" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">目的地
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA051" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">總店號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA065" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">總公司請款
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA066" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">分店數
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA067" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">型態別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA076" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">路線別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA077" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">其他別
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA078" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">發票地址-郵遞區號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA079" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">送貨地址-郵遞區號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA080" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">文件地址-郵遞區號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA081" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">帳單郵遞區號
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA098" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">帳單地址(一)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA099" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">帳單地址(二)
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA100" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableModifyTdHead">帳單收件人
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA101" runat="server"></asp:Literal>
                                </td>
                                <td class="TableModifyTdHead">稅別碼
                                </td>
                                <td class="TableModifyTd">
                                    <asp:Literal ID="lt_MA118" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="col-sm-2">
                <nav id="mySideMenu" class="hidden-print hidden-xs affix">
                    <ul class="nav nav-pills nav-stacked">
                        <li class="active"><a href="#baseData"><i class="fa fa-file fa-fw" aria-hidden="true"></i>&nbsp;基本資料</a></li>
                        <li><a href="#dbData"><i class="fa fa-database fa-fw" aria-hidden="true"></i>&nbsp;資料庫設定</a></li>
                        <li><a href="#rptData"><i class="fa fa-bar-chart fa-fw" aria-hidden="true"></i>&nbsp;報表設定</a></li>
                        <li><a href="#otherData"><i class="fa fa-file-text fa-fw" aria-hidden="true"></i>&nbsp;其他資料</a></li>
                        <li><a href="<%=Page_SearchUrl %>"><i class="fa fa-undo fa-fw" aria-hidden="true"></i>&nbsp;返回列表</a></li>
                    </ul>
                </nav>
            </div>
        </div>
    </form>
</body>
</html>
