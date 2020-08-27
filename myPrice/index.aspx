<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="myPrice_index" %>

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
                    $("#tb_CustID").val(ui.item.custId);
                }
            });

        });
    </script>
    <%-- Autocompelete End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/blockUI/customFunc.js"></script>
    <script>
        $(function () {
            //Click事件, 觸發
            $("#triggerGo").click(function () {
                //block-ui
                blockBox1('Select', '資料處理中...');

                //觸發
                $('#btn_Search').trigger('click');
            });

        });
    </script>
    <%-- blockUI End --%>
</head>
<body class="myBody">
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="page-header">
                        <h2>客戶報價
                    <small>
                        <a>業務行銷</a>&nbsp;
                        <i class="fa fa-chevron-right"></i>&nbsp;<span>客戶報價</span>
                    </small>
                        </h2>
                    </div>
                </div>
            </div>
            <!-- 查詢客戶報價 -->
            <div class="row">
                <div class="col-md-12">
                    <div class="bq-callout blue">
                        <h4>查詢客戶報價</h4>
                        <div class="form-group">
                            <label for="tb_CustName">選擇客戶</label>
                            <asp:TextBox ID="tb_CustName" runat="server" CssClass="form-control" placeholder="輸入關鍵字" aria-label="輸入關鍵字"></asp:TextBox>
                            <asp:TextBox ID="tb_CustID" runat="server" Style="display: none" ToolTip="客戶ERP代號"></asp:TextBox>
                            <div class="help-block">(請輸入關鍵字：客戶編號或名稱)</div>
                        </div>
                        <div class="btn-group btn-group-justified">
                            <div class="btn-group">
                                <button type="button" id="triggerGo" class="btn btn-primary">開&nbsp;始&nbsp;查&nbsp;詢</button>
                            </div>
                        </div>
                        <asp:RequiredFieldValidator ID="rfv_tb_CustID" runat="server" ErrorMessage="請選擇「客戶」" ControlToValidate="tb_CustID" CssClass="text-danger" Display="Dynamic" ValidationGroup="Select"></asp:RequiredFieldValidator>
                        <div class="hidden">
                            <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" ValidationGroup="Select" />
                        </div>
                    </div>
                </div>
            </div>
            <!-- 建立價格比較表 -->
            <div class="row">
                <div class="col-md-12">
                    <div class="bq-callout orange">
                        <h4>價格比較表&nbsp;<small>(報表中心)</small></h4>
                        <div class="btn-group btn-group-justified">
                            <a href="PriceCompare_Create.aspx" class="btn btn-warning">開&nbsp;始&nbsp;建&nbsp;立</a>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 已儲存的價格比較表 -->
            <div class="row">
                <div class="col-md-12">
                    <div class="bq-callout green">
                        <h4>已儲存的價格比較表</h4>
                        <div class="row">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="5">
                                <LayoutTemplate>
                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <div class="col-sm-6">
                                        <div class="list-group">
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </div>
                                    </div>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <a href="PriceCompare_Create.aspx?DataID=<%#Eval("ID") %>" class="list-group-item"><%#Eval("Label") %></a>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
