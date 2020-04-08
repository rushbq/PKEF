<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PriceCompare_Create.aspx.cs" Inherits="myPrice_PriceCompare_Create" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.1/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../css/layout.css?v=20160622" rel="stylesheet" type="text/css" />
    <script src="https://code.jquery.com/jquery-2.2.3.min.js" integrity="sha256-a23g1Nt4dtEYOj7bR+vTu7+T8VP13humZFBJNIYoEJo=" crossorigin="Proskit"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- bootstrap Start --%>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="Proskit" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="Proskit"></script>
    <%-- bootstrap End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/blockUI/customFunc.js"></script>
    <%-- blockUI End --%>
    <%-- Autocompelete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="../js/catcomplete/catcomplete.js"></script>
    <script>
        $(function () {
            /* Autocomplete - 客戶 */
            $("#tb_CustName").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Customer.aspx",
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
                                        value: "(" + item.custid + ") " + item.shortName,
                                        custId: item.custid,
                                        shortName: item.shortName
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_CustName").val(ui.item.label);
                    $("#tb_CustID").val(ui.item.custId);

                    event.preventDefault();
                }
            });

            /* Autocomplete - 品號 */
            $("#tb_ModelNo").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.id,
                                        category: item.category,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_ModelNo").val(ui.item.id);
                    $("#tb_ModelNo_Val").val(ui.item.id);

                    event.preventDefault();
                }
            });

        });
    </script>
    <%-- Autocompelete End --%>
</head>
<body class="myBody">
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="page-header">
                        <h2>價格比較表
                        <small>
                            <a>業務行銷</a>&nbsp;
                            <i class="fa fa-chevron-right"></i>&nbsp;<span>價格比較表</span>
                        </small>
                        </h2>
                    </div>
                </div>
            </div>
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <div class="pull-left">
                        <i class="fa fa-cog fa-spin" aria-hidde="true"></i>
                        <span>條件設定</span>
                    </div>
                    <div class="pull-right text-right">
                        <a href="PriceCompare_View.aspx?DataID=<%=Req_DataID %>" class="btn btn-primary"><i class="fa fa-table"></i>&nbsp;顯示比較表</a>
                        <asp:Button ID="btn_DelSheet" runat="server" Text="刪除" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除?')" OnClick="btn_DelSheet_Click" />
                        <a href="index.aspx" class="btn btn-default">返回首頁</a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <fieldset>
                                    <legend><small>自訂表格名稱</small></legend>
                                    <div class="form-inline">
                                        <asp:TextBox ID="tb_Subject" runat="server" CssClass="form-control" placeholder="填寫表格名稱" MaxLength="100" ToolTip="字數上限 50 字"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Subject" runat="server" ErrorMessage="請填寫「表格名稱」" ControlToValidate="tb_Subject" Display="Dynamic" ValidationGroup="Add" CssClass="text-danger"></asp:RequiredFieldValidator>

                                        <asp:Button ID="btn_AddName" runat="server" CssClass="btn btn-primary" Text="變更名稱" ValidationGroup="Add" OnClientClick="blockBox1('Add', '名稱變更中...');" OnClick="btn_AddName_Click" />
                                    </div>
                                </fieldset>
                            </div>
                        </div>

                        <hr />
                        <!-- 客戶 Start -->
                        <div class="bq-callout orange">
                            <h4>客戶名稱</h4>
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <div class="form-inline">
                                        <asp:TextBox ID="tb_CustName" runat="server" CssClass="form-control" placeholder="輸入關鍵字" aria-label="輸入關鍵字"></asp:TextBox>
                                        <asp:TextBox ID="tb_CustID" runat="server" Style="display: none"></asp:TextBox>

                                        <asp:Button ID="btn_AddCust" runat="server" CssClass="btn btn-warning" Text="加入" ValidationGroup="AddCust" OnClientClick="blockBox1('AddCust', '客戶加入中...');" OnClick="btn_AddCust_Click" />
                                        <asp:RequiredFieldValidator ID="rfv_tb_CustID" runat="server" ErrorMessage="請選擇「客戶」" ControlToValidate="tb_CustID" Display="Dynamic" ValidationGroup="AddCust" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div id="custList" class="table-responsive">
                                <asp:ListView ID="lv_CustList" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_CustList_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="table table-bordered table-striped">
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Group" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <GroupTemplate>
                                        <tr>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tr>
                                    </GroupTemplate>
                                    <ItemTemplate>
                                        <td style="width: 35%">
                                            <%#Eval("Label") %>&nbsp;(<%#Eval("ID") %>)
                                        </td>
                                        <td style="width: 15%" class="text-center">
                                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除關聯?')" ToolTip="刪除"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("ID") %>' />
                                        </td>
                                    </ItemTemplate>
                                    <EmptyItemTemplate>
                                        <td style="width: 35%">&nbsp;</td>
                                        <td style="width: 15%">&nbsp;</td>
                                    </EmptyItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="text-center text-danger" style="padding: 15px 0px 15px 0px;">
                                            <h4><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;尚未加入條件</h4>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <!-- 客戶 End -->

                        <hr />
                        <!-- 產品品號 Start -->
                        <div class="bq-callout green">
                            <h4>產品品號</h4>
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <div class="form-inline">
                                        <asp:TextBox ID="tb_ModelNo" runat="server" CssClass="form-control" placeholder="輸入品號關鍵字" aria-label="輸入品號關鍵字"></asp:TextBox>
                                        <asp:TextBox ID="tb_ModelNo_Val" runat="server" Style="display: none"></asp:TextBox>

                                        <asp:Button ID="btn_AddProd" runat="server" CssClass="btn btn-success" Text="加入" ValidationGroup="AddProd" OnClientClick="blockBox1('AddProd', '品號加入中...');" OnClick="btn_AddProd_Click" />
                                        <asp:RequiredFieldValidator ID="rfv_tb_ModelNo" runat="server" ErrorMessage="請填寫「產品品號」" ControlToValidate="tb_ModelNo_Val" Display="Dynamic" ValidationGroup="AddProd" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div id="prodList" class="table-responsive">
                                <asp:ListView ID="lv_Prod" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_Prod_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="table table-bordered table-striped">
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Group" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <GroupTemplate>
                                        <tr>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tr>
                                    </GroupTemplate>
                                    <ItemTemplate>
                                        <td style="width: 35%">
                                            <%#Eval("ID") %>
                                        </td>
                                        <td style="width: 15%" class="text-center">
                                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除關聯?')" ToolTip="刪除"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("ID") %>' />
                                        </td>
                                    </ItemTemplate>
                                    <EmptyItemTemplate>
                                        <td style="width: 35%">&nbsp;</td>
                                        <td style="width: 15%">&nbsp;</td>
                                    </EmptyItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="text-center text-danger" style="padding: 15px 0px 15px 0px;">
                                            <h4><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;尚未加入條件</h4>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <!-- 產品品號 End -->
                    </div>
                </div>
            </div>
            <div class="well well-sm">
                <h4>說明</h4>
                <ul>
                    <li>進入此頁後, 系統即建立一筆新的表格, 表格命名可變更。</li>
                    <li>「客戶名稱」與「產品品號」條件加入完畢，即可按下「顯示比較表」。</li>
                    <li>建立過的比較表, 皆會在首頁顯示。</li>
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
