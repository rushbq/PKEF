<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Write.aspx.cs" Inherits="WR_Write" %>

<%@ Import Namespace="ExtensionMethods" %>
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
    <%-- smallipop Start --%>
    <link href="../js/smallipop/css/contrib/animate.min.css" rel="stylesheet" type="text/css" />
    <link href="../js/smallipop/css/jquery.smallipop.min.css" rel="stylesheet" type="text/css" />
    <script src="../js/smallipop/jquery.smallipop.min.js" type="text/javascript"></script>
    <script src="../js/smallipop/modernizr.min.js" type="text/javascript"></script>
    <%-- smallipop End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //勾選待辦事項
            $('input[id*="cb_Confirm"]').click(function () {
                //取得上層 - TR
                var this_tID = $(this).parent().parent("tr");
                //[取得屬性] - value (Task_ID)
                var taskID = $(this).attr("value");
                //[取得屬性] - span (符號表示欄)
                var checkIcon = $(this_tID).find('span.check');

                //定義是否選取
                var isChecked = "";
                if ($(this).prop("checked")) {
                    this_tID.addClass('TrYellow').css('text-decoration', 'line-through');
                    isChecked = "Y";
                } else {
                    this_tID.removeClass('TrYellow').css('text-decoration', 'none');
                    isChecked = "N";
                }

                $.ajax({
                    url: 'WR_Write_Ajax.aspx' + '?' + new Date().getTime(),
                    data: {
                        ValidCode: '<%=ValidCode %>',
                        taskID: taskID,
                        isChecked: isChecked
                    },
                    type: "POST",
                    dataType: "html"

                }).done(function (html) {
                    if (html.indexOf("OK") != -1) {
                        //成功
                        if (isChecked == "Y") {
                            //顯示符號
                            checkIcon.html('<span class="JQ-ui-state-highlight"><span class="JQ-ui-icon ui-icon-check"></span></span>');
                        } else {
                            checkIcon.html('');
                        }

                    } else {
                        alert('資料處理失敗，請聯絡系統管理員!');
                    }

                }).fail(function (jqXHR, textStatus) {
                    alert('資料處理失敗，請聯絡系統管理員!' + textStatus);
                });

            });

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
    <%-- highslide Start --%>
    <script type="text/javascript" src="../js/highslide/highslide-with-html.packed.js"></script>
    <link rel="stylesheet" type="text/css" href="../js/highslide/highslide.css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $(".showEdit").click(function () {
                var thisSrc = $(this).attr("rel");
                hs.graphicsDir = '../js/highslide/graphics/';
                hs.outlineType = 'rounded-white';
                hs.showCredits = false;
                hs.wrapperClassName = 'draggable-header';
                hs.outlineWhileAnimating = true;
                hs.preserveContent = false;
                hs.htmlExpand(this, {
                    src: thisSrc,
                    headingText: '工作項目維護',
                    objectType: 'iframe',
                    width: 700
                });
            });

            $(".showMore").click(function () {
                var thisHead = $(this).attr("head");
                hs.graphicsDir = '../js/highslide/graphics/';
                hs.outlineType = 'rounded-white';
                hs.showCredits = false;
                hs.wrapperClassName = 'draggable-header';
                hs.htmlExpand(this, {
                    headingText: thisHead
                });
            });

        });
    </script>
    <%-- highslide End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>工作日誌</a>&gt;<span>填寫日誌</span>
        </div>
        <div class="h2Head">
            <div class="title">
                <div class="pull-left head">
                    填寫日誌 -
                <asp:Label ID="lb_showToday" runat="server" CssClass="B styleBlue tipTour" ToolTip="今天的日期"
                    data-smallipop-tour-index="1"></asp:Label>
                </div>
                <div class="pull-right">
                    <input type="button" class="btnBlock colorBlue tipTour showEdit" value="新增項目" rel="WR_Write_Edit.aspx"
                        title="按下「新增項目」，新增今日的工作項目" data-smallipop-tour-index="2" />
                    <input type="button" id="runTour" value="?" title="導覽" class="btnBlock colorDark" />
                </div>
                <div class="clearfix"></div>
            </div>
        </div>

        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <tr class="tdHead">
                            <td width="40px"></td>
                            <td width="20px"></td>
                            <td width="350px">工作項目
                            </td>
                            <td>內容說明
                            </td>
                            <td width="120px">&nbsp;
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td align="center">
                            <asp:CheckBox ID="cb_Confirm" runat="server" />
                        </td>
                        <td align="center">
                            <asp:Label ID="lb_MsgIcon" runat="server" CssClass="check"></asp:Label>
                        </td>
                        <td>
                            <div class="L2MainHead">
                                <%#Eval("Task_Name")%>
                            </div>
                            <div class="styleGraylight Font13" style="padding-top:8px;">
                                <span class="styleGreen"><%#Eval("Class_Name")%></span><asp:Label ID="lb_pastTime" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td>
                            <a class="showMore styleGraylight" style="cursor: pointer" title="展開" head="<%#Eval("Task_Name")%>">
                                <%# fn_stringFormat.StringLimitOutput(Eval("Remark").ToString().Replace("\r","<br/>"),50, fn_stringFormat.WordType.字數, true) %>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                            <div class="highslide-maincontent">
                                <%# Eval("Remark").ToString().Replace("\r","<br/>") %>
                                <div style="text-align: right; padding-top: 5px;">
                                    <input type="button" value="關閉" onclick="return hs.close(this);" class="btnBlock colorGray" />
                                </div>
                            </div>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="btnBlock colorBlue showEdit" style="cursor: pointer;"
                                    rel="WR_Write_Edit.aspx?Task_ID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Task_ID").ToString()))%>"><span class="glyphicon glyphicon-pencil"></span>&nbsp;修改</a>
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div style="padding: 120px 0px 120px 0px; text-align: center">
                        <span style="color: #FD590B; font-size: 12px">今日尚未新增工作項目！</span>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div class="ListIllusArea">
            <div class="JQ-ui-state-default">
                <span class="JQ-ui-icon ui-icon-info"></span>&nbsp;<span class="B">背景色代表意義：</span><br />
                <table class="List1">
                    <tr class="TrOrange">
                        <td class="JQ-ui-state-error">
                            <span class="JQ-ui-icon ui-icon-notice"></span>未完成項目
                        </td>
                    </tr>
                </table>
                <table class="List1">
                    <tr class="TrYellow">
                        <td class="JQ-ui-state-highlight">
                            <span class="JQ-ui-icon ui-icon-check"></span>已勾選完成
                        </td>
                    </tr>
                </table>
                <table class="List1">
                    <tr>
                        <td>今日工作項目
                        </td>
                    </tr>
                </table>
                <table class="List1">
                    <tr class="TrBlue">
                        <td class="JQ-ui-state-default">
                            <span class="JQ-ui-icon ui-icon-clock"></span>未來工作項目
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
