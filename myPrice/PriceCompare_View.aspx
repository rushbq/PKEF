<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PriceCompare_View.aspx.cs" Inherits="myPrice_PriceCompare_View" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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
    <%-- bootstrap Start --%>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="Proskit" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="Proskit"></script>
    <%-- bootstrap End --%>
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
                            <i class="fa fa-chevron-right"></i>&nbsp;<a href="<%=Page_PrevUrl %>">條件設定</a>&nbsp;
                            <i class="fa fa-chevron-right"></i>&nbsp;<span><asp:Literal ID="lt_TableName" runat="server"></asp:Literal></span>
                        </small>
                        </h2>
                    </div>
                </div>
            </div>
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <i class="fa fa-table" aria-hidde="true"></i>
                        <span>顯示結果</span>
                    </div>
                    <div class="pull-right text-right">
                        <a href="<%=Page_PrevUrl %>" class="btn btn-success">返回條件設定</a>
                        <a href="<%=Page_SearchUrl %>" class="btn btn-default">返回首頁</a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <div style="width: auto; padding-top: 5px;">
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>
                        <rsweb:ReportViewer ID="RptData" runat="server" Width="100%" Height="100%" AsyncRendering="False" SizeToReportContent="True">
                        </rsweb:ReportViewer>
                    </div>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
