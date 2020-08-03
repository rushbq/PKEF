<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IT_HelpView.aspx.cs" Inherits="IT_HelpView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
            });

        });


    </script>
    <%-- raty 評分 Start --%>
    <script src="../js/rate/jquery.raty.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            /*
                jQuery評分 - raty
                參考:http://wbotelhos.com/raty
            */
            //未評分
            $('#rateMe').raty({
                starOff: '<%=Application["WebUrl"]%>js/rate/img/star-off-big.png',
                starOn: '<%=Application["WebUrl"]%>js/rate/img/star-on-big.png',
                hints: ['太遜了', '勉強接受', '普普通通', '還不錯', '太棒了'],
                target: '#rateHint',
                click: function (score, evt) {
                    $.ajax({
                        url: '<%=Application["WebUrl"]%>Rate_Ajax.aspx' + '?' + new Date().getTime(),
                        data: {
                            ValidCode: '<%=ValidCode %>',
                            dataID: '<%=Param_thisID%>',
                            score: score,
                            ratetype: 'ITHelp'
                        },
                        type: "POST",
                        dataType: "html"

                    }).done(function (html) {
                        if (html.indexOf("OK") != -1) {
                            //成功
                            $('#rateHint').text("評分完成!");
                        } else {
                            $('#rateHint').text("評分失敗....");
                        }

                    }).fail(function (jqXHR, textStatus) {
                        alert('資料處理失敗，請聯絡系統管理員!' + textStatus);
                    });

                }
            });

            //已評分
            $('#rateShow').raty({
                starOff: '<%=Application["WebUrl"]%>js/rate/img/star-off-big.png',
                starOn: '<%=Application["WebUrl"]%>js/rate/img/star-on-big.png',
                hints: ['太遜了', '勉強接受', '普普通通', '還不錯', '太棒了'],
                target: '#rateHint',
                targetKeep: true,
                readOnly: true,
                score: '<%=Param_Score%>'
            });

        });
    </script>
    <%-- raty 評分 End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>需求記錄表</a>&gt;<span>需求登記</span>
        </div>
        <div class="h2Head">
            <h2>需求登記</h2>
        </div>
        <table class="TableModify">
            <!-- 主檔 Start -->
            <tr class="ModifyHead">
                <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTdHead" style="width: 120px">追蹤編號
                    </td>
                    <td class="TableModifyTd styleBlue B" style="width: 400px;">
                        <asp:Literal ID="lt_TraceID" runat="server">系統自動編號</asp:Literal>
                    </td>
                    <td class="TableModifyTdHead" style="width: 120px">處理狀態
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Help_Status" runat="server" Text="資料新增中" CssClass="styleGraylight"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">問題類別
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Req_Class" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">報修方式
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Help_Way" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">需求者
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_ReqWho" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">填寫主旨
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Help_Subject" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="min-height: 25px;">有圖有真相
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <div>
                            <asp:ListView ID="lvDataList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="4">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td align="center" valign="top" width="25%">
                                        <div>
                                            <%#PicUrl(Eval("AttachFile").ToString(), Eval("AttachFile_Org").ToString())%>
                                        </div>
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td></td>
                                </EmptyItemTemplate>
                            </asp:ListView>
                        </div>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">詳細說明
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Help_Content" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                </tr>
                <asp:PlaceHolder ID="ph_Agree" runat="server" Visible="false">
                    <tr>
                        <td class="TableModifyTdHead">主管同意
                        </td>
                        <td class="TableModifyTd">
                            <asp:Label ID="lb_IsAgree" runat="server" CssClass="stylePurple B"></asp:Label>
                        </td>
                        <td class="TableModifyTdHead">同意者/時間
                        </td>
                        <td class="TableModifyTd styleGreen">
                            <asp:Literal ID="lt_Agree_Who" runat="server"></asp:Literal>&nbsp;&nbsp;|&nbsp;&nbsp;
                            <asp:Literal ID="lt_Agree_Time" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </tbody>
            <tbody>
                <tr class="ModifyHead">
                    <td colspan="4">回覆資料<em class="TableModifyTitleIcon"></em>&nbsp;
                        <asp:PlaceHolder ID="ph_Reply" runat="server" Visible="false">
                            <a class="btnBlock colorRed" href="IT_HelpEdit.aspx?TraceID=<%= Server.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey))%>">我要回覆
                            </a>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Showrate" runat="server" Visible="false">
                    <tr class="Must">
                        <td class="TableModifyTdHead" style="min-height: 25px;"><em>(*)</em> 滿意度評分</td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:PlaceHolder ID="pl_Rate" runat="server">
                                <span id="rateMe"></span>
                                <%--<span class="SiftLight" style="padding-left: 5px;">(若未評分則預設為滿分)</span>--%>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="pl_ShowRate" runat="server">
                                <span id="rateShow"></span>
                            </asp:PlaceHolder>

                            <span id="rateHint" class="styleBlue" style="padding-left: 5px;"></span>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="TableModifyTdHead" style="min-height: 25px;">回覆的附件
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:ListView ID="lvDataList_Reply" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                            GroupItemCount="4">
                            <LayoutTemplate>
                                <table class="List1" width="100%">
                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                </table>
                            </LayoutTemplate>
                            <GroupTemplate>
                                <tr>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tr>
                            </GroupTemplate>
                            <ItemTemplate>
                                <td align="center" valign="top" width="25%">
                                    <%#PicUrl(Eval("AttachFile").ToString(), Eval("AttachFile_Org").ToString())%>
                                </td>
                            </ItemTemplate>
                            <EmptyItemTemplate>
                                <td></td>
                            </EmptyItemTemplate>
                        </asp:ListView>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="min-height: 25px;">處理工時
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Reply_Hours" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="min-height: 25px;">處理回覆
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Reply_Content" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="min-height: 25px;">回覆日期
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Reply_Date" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>

            <!-- 主檔 End -->
            <!-- 維護資訊 Start -->
            <tr class="ModifyHead">
                <td colspan="4">維護資訊<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 100px">維護資訊
                </td>
                <td class="TableModifyTd" colspan="3">
                    <table cellpadding="3" border="0">
                        <tr>
                            <td align="right" width="100px">建立者：
                            </td>
                            <td class="styleGreen" width="200px">
                                <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                            <td align="right" width="100px">建立時間：
                            </td>
                            <td class="styleGreen" width="250px">
                                <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">最後修改者：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Who" runat="server"></asp:Literal>
                            </td>
                            <td align="right">最後修改時間：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Time" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!-- 維護資訊 End -->
        </table>
        <div class="SubmitAreaS">
            <a href="<%=Page_SearchUrl %>" class="btnBlock colorGray">返回列表</a>
            <asp:Button ID="btn_Done" runat="server" Text="已自行解決，謝謝" OnClick="btn_Done_Click"
                CssClass="btnBlock colorRed" OnClientClick="return confirm('狀態將會設為「已結案」\n\r是否確定送出?')" Visible="false" />
        </div>
    </form>
</body>
</html>
