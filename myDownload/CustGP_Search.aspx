<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustGP_Search.aspx.cs" Inherits="CustGP_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../js/jquery.js"></script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <link href="../css/font-awesome.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Filter Start -->
        <div class="row">
            <div class="col-sm-12">
                <div class="panel panel-warning">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-filter"></span>
                            <span>篩選器</span>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#filter">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="filter" class="collapse in">
                        <div class="panel-body">
                            <!-- Filter Content Start -->
                            <div class="row">
                                <div class="col-sm-6">
                                    <label>關鍵字查詢</label>
                                    <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="20"></asp:TextBox>
                                </div>
                                <div class="col-sm-6">
                                    <div class="text-right">
                                        <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" />
                                        <a href="CustGP_Edit.aspx" class="btn btn-primary">新增群組</a>
                                    </div>
                                </div>
                            </div>
                            <!-- Filter Content End -->
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Filter End -->
        <!-- Result Start -->
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-success">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-list"></span>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#result">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <!-- Table Content Start -->
                    <div id="result" class="table-responsive collapse in">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                            <LayoutTemplate>
                                <table class="table table-bordered table-advance table-striped">
                                    <thead>
                                        <tr>
                                            <th style="width: 10%">編號</th>
                                            <th>名稱</th>
                                            <th style="width: 15%">是否顯示</th>
                                            <th style="width: 15%">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="text-center">
                                        <%#Eval("Group_ID") %>
                                    </td>
                                    <td>
                                        <%#Eval("Group_Name") %>
                                    </td>
                                    <td class="text-center">
                                        <%#Eval("Display") %>
                                    </td>
                                    <td class="text-center">
                                        <a href="CustGP_Edit.aspx?DataID=<%#HttpUtility.UrlEncode(Eval("Group_ID").ToString()) %>" class="btn btn-primary">
                                            <i class="fa fa-pencil fa-lg"></i>
                                        </a>&nbsp;
                                        <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger delete" OnClientClick="return confirm('確定要刪除這筆資料嗎？')">
                                            <i class="fa fa-trash-o fa-lg"></i>
                                        </asp:LinkButton>

                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Group_ID") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="text-center styleReddark" style="padding: 60px 0px 60px 0px;">
                                    <h3><span class="glyphicon glyphicon-exclamation-sign"></span>&nbsp;查無資料</h3>
                                </div>
                            </EmptyDataTemplate>

                        </asp:ListView>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
        </div>
        <!-- Result End -->
    </form>
</body>
</html>
