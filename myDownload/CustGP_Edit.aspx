<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustGP_Edit.aspx.cs" Inherits="CustGP_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery.js"></script>
    <!-- 動態欄位js Start -->
    <script src="../js/public.js"></script>
    <script src="../js/dynamic-ListItem.js"></script>
    <!-- 動態欄位js End -->
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/blockUI/customFunc.js"></script>
    <%-- blockUI End --%>
    <%-- Autocompelete Start --%>
    <script>
        $(function () {
            /* Autocomplete - 品號 */
            $("#tb_myFilterItem").autocomplete({
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
                                        value: item.shortName,
                                        custId: item.custid,
                                        shortName: item.shortName
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    //呼叫動態欄位, 新增項目
                    Add_Item_Normal("myItemList", ui.item.custId, ui.item.value, true);

                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

        });

    </script>
    <%-- Autocompelete End --%>
    <!-- 共用js -->
    <script type="text/javascript">
        $(document).ready(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                //block-ui
                blockBox1('Add', '資料處理中...');

                //取得動態欄位資料 - 客戶
                Get_Item('myItemList', 'myValues', 'Item_ID');

                //觸發
                $('#btn_doSave').trigger('click');
            });

            //判斷是否為修改資料, 將已選擇的選項active
            <% if (!string.IsNullOrEmpty(Param_thisID))
               { %>
            var myTarObj = $("input:checked[name='rbl_Display']");
            var getVal = myTarObj.val();
            if (getVal != undefined) {
                myTarObj.parent().addClass("active");
            }
            <% } %>

        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-edit"></span>
                            <span>經銷商群組設定</span>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#data">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="data" class="collapse in">
                        <div class="panel-body">
                            <!-- Content Start -->
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="control-label col-sm-2">群組名稱</label>
                                    <div class="col-sm-10">
                                        <asp:TextBox ID="tb_Group_Name" runat="server" CssClass="form-control tip" placeholder="群組名稱,最多70字" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Group_Name" runat="server" ErrorMessage="請填寫「群組名稱」" ControlToValidate="tb_Group_Name" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-sm-2">顯示狀態</label>
                                    <div class="col-sm-10">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="btn-group" data-toggle="buttons">
                                            <asp:ListItem Value="Y" Selected="True" class="btn btn-default">顯示</asp:ListItem>
                                            <asp:ListItem Value="N" class="btn btn-default">隱藏</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-sm-2">排序</label>
                                    <div class="col-sm-10">
                                        <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                                            Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                            Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                            ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-sm-2">設定客戶名單</label>
                                    <div class="col-sm-10">
                                        <div class="form-group">
                                            <div class="help-block">請輸入關鍵字：客戶代號或名稱</div>
                                            <asp:TextBox ID="tb_myFilterItem" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfv_myValues" runat="server" ErrorMessage="請設定「客戶名單」" ControlToValidate="myValues" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group">
                                            <div data-rel="data-list">
                                                <ul class="list-group" id="myItemList">
                                                    <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                                </ul>

                                                <asp:TextBox ID="myValues" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                                </asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-sm-12 text-right">
                                        <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />

                                        <button type="button" id="triggerSave" class="btn btn-primary">
                                            <asp:Literal ID="lt_Save" runat="server">新增資料</asp:Literal></button>
                                        <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
                                        <button type="button" onclick="location.href='CustGP_Search.aspx'" class="btn btn-default">返回列表</button>
                                    </div>
                                </div>
                            </div>
                            <!-- Content End -->
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
