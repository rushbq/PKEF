<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Msg_View.aspx.cs" Inherits="Msg_View" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <script>
        //返回列表
        function goToList() {
            location.href = '<%=Session["BackListUrl"] %>';
        }

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>業務行銷</a>&gt;<span>官網網站訊息</span>
        </div>
        <div class="h2Head">
            <h2>官網網站訊息</h2>
        </div>
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <!-- 主檔 Start -->
                <tr class="ModifyHead">
                    <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 150px">追蹤編號
                        </td>
                        <td class="TableModifyTd styleBlue B" style="width: 400px">
                            <asp:Literal ID="lt_TraceID" runat="server">系統自動編號</asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">訊息類別 / 處理狀態
                        </td>
                        <td class="TableModifyTd">
                            <asp:Label ID="lb_Class" runat="server" CssClass="label label-danger"></asp:Label>
                            <asp:Label ID="lb_Status" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead"><strong>留言訊息</strong>
                        </td>
                        <td class="TableModifyTd bg-warning" colspan="3"><strong>
                            <asp:Literal ID="lt_Message" runat="server"></asp:Literal></strong>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">回覆信收件者
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Label ID="lb_AreaName" runat="server" CssClass="label label-default"></asp:Label>&nbsp;
                            <asp:Literal ID="lt_MemberMail" runat="server"></asp:Literal>
                            &nbsp;
                            <a href="javascript:void(0)" data-toggle="modal" data-target="#myModal">顯示會員資料&nbsp;<span class="glyphicon glyphicon-new-window"></span></a>

                            <!-- 會員資料 Modal Start -->
                            <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <h4 class="modal-title" id="myModalLabel">會員資料</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row form-horizontal">
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">Email</label>
                                                    <div class="col-xs-10 form-control-static">
                                                        <asp:Literal ID="modal_Email" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">公司</label>
                                                    <div class="col-xs-10 form-control-static">
                                                        <asp:Literal ID="modal_Company" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">生日</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Birthday" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">性別</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Sex" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">姓</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_FirstName" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">名</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_LastName" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">電話</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Tel" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">手機</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Mobile" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">QQ</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_qq" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">WeChat</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_wechat" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">國家</label>
                                                    <div class="col-xs-10 form-control-static">
                                                        <asp:Literal ID="modal_Country" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">地址</label>
                                                    <div class="col-xs-10 form-control-static">
                                                        <asp:Literal ID="modal_Address" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- 會員資料 Modal End -->
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">回覆信寄件者
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_ReplyMailSender" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <!-- 轉寄欄位 Start -->
                    <tr class="Must">
                        <td class="TableModifyTdHead">回覆信轉寄對象<br />
                            <em class="styleRed">(不會收到客戶資料)</em></td>
                        <td class="TableModifyTd" colspan="3">
                            <div class="showResult">
                                <ul class="list-inline" id="myEmpItemView">
                                    <asp:Literal ID="lt_EmpItems" runat="server"></asp:Literal>
                                </ul>
                                <ul class="list-inline" id="myOtherItemView">
                                    <asp:Literal ID="lt_OtherItems" runat="server"></asp:Literal>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <!-- 轉寄欄位 End -->
                    <tr class="Must">
                        <td class="TableModifyTdHead">信件主旨</td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_Subject" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr class="Must">
                        <td class="TableModifyTdHead">回覆內文</td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_Reply_Message" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <!-- 轉寄承辦人員 Start -->
                    <tr class="Must">
                        <td class="TableModifyTdHead">轉寄承辦人員<br />
                            <em class="styleRed">(將會收到客戶資料)</em></td>
                        <td class="TableModifyTd" colspan="3">
                            <ul class="list-inline" id="mySalesItemView">
                                <asp:Literal ID="lt_SalesItems" runat="server"></asp:Literal>
                            </ul>
                        </td>
                    </tr>
                    <!-- 轉寄承辦人員 End -->
                    <tr>
                        <td class="TableModifyTdHead">維護資訊
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <table cellpadding="3" border="0">
                                <tr>
                                    <td align="right" width="100px">建立時間：
                                    </td>
                                    <td class="styleGreen" width="200px">
                                        <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                                    </td>
                                    <td align="right" width="100px">回覆時間：
                                    </td>
                                    <td class="styleGreen" width="250px">
                                        <asp:Literal ID="lt_Reply_Time" runat="server" Text="新增資料中"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </tbody>

                <!-- 主檔 End -->
            </table>
        </div>
        <div class="SubmitAreaS">
            <input onclick="goToList();" type="button" value="返回列表" class="btnBlock colorGray" />
        </div>

        <!-- 將來的討論串 -->
        <%--<div class="bq-callout orange">
            <div>
                <p>
                    okkki
                </p>
                <footer>2014/01/30 11:25</footer>
            </div>
        </div>--%>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
