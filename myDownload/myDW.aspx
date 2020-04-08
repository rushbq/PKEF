<%@ Page Language="C#" AutoEventWireup="true" CodeFile="myDW.aspx.cs" Inherits="myDW" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
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
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="../js/catcomplete/catcomplete.js"></script>
    <script>
        $(function () {
            /* Autocomplete - 品號 */
            $("#tb_myFilterItem").catcomplete({
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
                                        label: "(" + item.id + ") " + item.label,
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
                    //呼叫動態欄位, 新增項目
                    Add_Item_Normal("myItemList", ui.item.id, ui.item.value, true);

                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

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
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox
            $(".infoBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none',
                afterClose: function () { // 關閉後自動reload
                    parent.mainFrame.location.reload(true);
                }
            });
        });
    </script>
    <%-- fancybox End --%>
    <!-- 動態選項js -->
    <script>
        $(document).ready(function () {
            //下載對象 Start --
            $("input[name='rbl_Target']").change(function () {
                var val = this.value;

                Check_Target(val);
            });

            //判斷是否為修改資料, 將已選擇的選項active
            <% if (!string.IsNullOrEmpty(Param_thisID))
               { %>
            var myTarObj = $("input:checked[name='rbl_Target']");
            var getVal = myTarObj.val();
            if (getVal != undefined) {
                myTarObj.parent().addClass("active");

                Check_Target(getVal);
            }
            <% } %>

            function Check_Target(myVal) {
                var showGP = $("#showSet_GP");
                var showOne = $("#showSet_One");

                showGP.hide();
                showOne.hide();

                switch (myVal) {
                    case "3":
                        showGP.show();
                        break;

                    case "4":
                        showOne.show();
                        break;

                    default:
                        showGP.hide();
                        showOne.hide();
                }
            }
            //下載對象 End --

            //經銷群組下拉change
            $("#ddl_CustGroup").change(function () {
                //取得值
                var GetValID = $(this).find(':selected').val();

                //取得文字
                var GetValName = $(this).find('option:selected').text();

                if (GetValID != '') {
                    //呼叫動態欄位, 新增項目
                    Add_Item_Normal("myGPList", GetValID, GetValName, false);
                }

            });
        });
    </script>
    <!-- 共用js -->
    <script type="text/javascript">
        $(document).ready(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                //block-ui
                blockBox1('Add', '資料處理中...');

                //取得動態欄位資料 - 品號
                Get_Item('myItemList', 'myModelValues', 'Item_ID');
                //取得動態欄位資料 - 客戶群組
                Get_Item('myGPList', 'myGPValues', 'Item_ID');

                //觸發
                $('#btn_doSave').trigger('click');
            });


            //Enter偵測
            $("#tb_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#btn_ListSearch").trigger("click");
                    //避免觸發submit
                    e.preventDefault();
                }
            });

        });
    </script>
    <style>
        .ui-autocomplete {
            z-index: 100 !important;
        }
    </style>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>業務行銷</a>&gt;<span>官網下載</span>
        </div>
        <!-- Step 1 -->
        <asp:PlaceHolder ID="ph_Class" runat="server">
            <div class="row">
                <div class="col-md-9">
                    <h3>&nbsp;1. 選擇分類</h3>
                    <asp:RadioButtonList ID="rbl_Class" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="rfv_rbl_Class" runat="server" ControlToValidate="rbl_Class"
                        Display="Dynamic" ErrorMessage="請選擇分類" ValidationGroup="Select" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                </div>
                <div class="col-md-9">
                    <h3>&nbsp;2. 選擇語系</h3>
                    <asp:RadioButtonList ID="rbl_LangType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="rfv_rbl_LangType" runat="server" ControlToValidate="rbl_LangType"
                        Display="Dynamic" ErrorMessage="請選擇語系" ValidationGroup="Select" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row" style="margin-top: 50px;">
                <div class="col-md-9">
                    <div class="btn-group btn-group-justified">
                        <asp:LinkButton ID="btn_Search" runat="server" CssClass="btn btn-success" OnClick="btn_Search_Click" ValidationGroup="Select">下 一 步</asp:LinkButton>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>

        <!-- Step 2 -->
        <asp:PlaceHolder ID="ph_Data" runat="server" Visible="false">
            <div class="alert alert-info alert-dismissible" role="alert">
                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                目前模式：<strong><asp:Literal ID="lt_Mode" runat="server">建立新檔案</asp:Literal></strong>
            </div>
            <!-- Data Insert -->
            <div class="table-responsive">
                <table class="TableModify table table-bordered">
                    <tbody>
                        <tr>
                            <td class="TableModifyTdHead">分類/語系
                            </td>
                            <td class="TableModifyTd Font20">
                                <span class="label label-danger">
                                    <asp:Literal ID="lt_ClassName" runat="server"></asp:Literal></span>&nbsp;
                                <span class="label label-warning">
                                    <asp:Literal ID="lt_LangTypeName" runat="server"></asp:Literal></span>
                            </td>
                        </tr>

                        <!-- 下載對象 Start -->
                        <tr>
                            <td class="TableModifyTdHead">下載對象</td>
                            <td class="TableModifyTd">
                                <div class="showRadioGrp">
                                    <asp:RadioButtonList ID="rbl_Target" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                </div>
                                <asp:RequiredFieldValidator ID="rfv_rbl_Target" runat="server" ControlToValidate="rbl_Target"
                                    Display="Dynamic" ErrorMessage="請選擇「下載對象」" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <!--群組經銷商設定-->
                        <tr id="showSet_GP" style="display: none;">
                            <td class="TableModifyTdHead">對象設定&nbsp;<i class="fa fa-level-up"></i></td>
                            <td class="TableModifyTd">
                                <div class="form-group form-inline">
                                    <asp:DropDownList ID="ddl_CustGroup" runat="server" CssClass="form-control"></asp:DropDownList>
                                    &nbsp;<a href="CustGP_Search.aspx" class="infoBox" title="經銷商群組維護"><i class="fa fa-exclamation-circle"></i>&nbsp;找不到群組?請點我新增</a>
                                </div>
                                <div class="form-group">
                                    <div data-rel="data-list">
                                        <ul class="list-group" id="myGPList">
                                            <asp:Literal ID="lt_GPList" runat="server"></asp:Literal>
                                        </ul>

                                        <asp:TextBox ID="myGPValues" runat="server" ToolTip="群組欄位值集合" Style="display: none;">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <!--指定經銷商設定-->
                        <tr id="showSet_One" style="display: none;">
                            <td class="TableModifyTdHead">對象設定&nbsp;<i class="fa fa-level-up"></i></td>
                            <td class="TableModifyTd">
                                <div class="help-block">請輸入關鍵字：客戶編號或名稱</div>
                                <asp:TextBox ID="tb_CustName" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:TextBox ID="tb_CustID" runat="server" Style="display: none" ToolTip="客戶ERP代號"></asp:TextBox>
                            </td>
                        </tr>
                        <!-- 下載對象 End -->
                        <tr>
                            <td class="TableModifyTdHead">檔案分類
                            </td>
                            <td class="TableModifyTd form-inline">
                                <asp:DropDownList ID="ddl_Type" runat="server" CssClass="form-control"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="TableModifyTdHead">顯示檔名
                            </td>
                            <td class="TableModifyTd form-inline">
                                <div class="input-group">
                                    <asp:TextBox ID="tb_DwFileName" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                                    <span class="input-group-addon">
                                        <asp:Literal ID="lt_FileExt" runat="server">副檔名自動產生</asp:Literal></span>
                                </div>
                                <div>
                                    <asp:RequiredFieldValidator ID="rfv_tb_DwFileName" runat="server" ControlToValidate="tb_DwFileName"
                                        Display="Dynamic" ErrorMessage="請填寫「顯示檔名」" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rev_tb_DwFileName" runat="server" ErrorMessage="輸入格式不正確(ex:Hello_v123)" ValidationExpression="\w+((_)?\w+)*" ControlToValidate="tb_DwFileName" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                    <div class="help-block">
                                        (字數限制:1~30個字 ; 輸入格式:英文+數字+底線，<code>無須填寫副檔名</code>，<code>檔案上限:200MB</code>)
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="TableModifyTdHead">檔案上傳
                            </td>
                            <td class="TableModifyTd">
                                <div>
                                    <asp:FileUpload ID="fu_Files" runat="server" />
                                    <asp:HiddenField ID="hf_myFileName" runat="server" />
                                    <asp:HiddenField ID="hf_OldFile" runat="server" />
                                </div>
                                <div>
                                    <asp:PlaceHolder ID="ph_files" runat="server" Visible="false">
                                        <div style="padding-top: 10px;">
                                            <a href="<%=Application["WebUrl"] %>Ashx_FtpFileDownload.ashx?dwFolder=<%=UploadFolder %>&realFile=<%=this.hf_OldFile.Value %>&dwFileName=<%=this.hf_myFileName.Value %>" class="btn btn-default"><i class="fa fa-cloud-download"></i>&nbsp;下載檔案</a>
                                        </div>
                                    </asp:PlaceHolder>
                                    <asp:RequiredFieldValidator ID="rfv_fu_Files" runat="server" ControlToValidate="fu_Files"
                                        Display="Dynamic" ErrorMessage="請上傳檔案" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                                <div class="help-block">
                                    (<code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>)
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="TableModifyTdHead">品號關聯</td>
                            <td class="TableModifyTd">
                                <div class="form-group">
                                    <div class="help-block">請輸入關鍵字：品號或品名</div>
                                    <asp:TextBox ID="tb_myFilterItem" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <div data-rel="data-list">
                                        <ul class="list-group" id="myItemList">
                                            <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                        </ul>

                                        <asp:TextBox ID="myModelValues" runat="server" ToolTip="品號欄位值集合" Style="display: none;">
                                        </asp:TextBox>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="TableModifyTd text-right bg-warning" colspan="2">
                                <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />

                                <button type="button" id="triggerSave" class="btn btn-primary">
                                    <asp:Literal ID="lt_Save" runat="server">新增檔案</asp:Literal></button>
                                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
                                <button type="button" onclick="location.href='myDW.aspx'" class="btn btn-default">重選分類</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Data List -->
            <div class="Sift form-inline">
                <ul>
                    <li>下載對象：<asp:DropDownList ID="ddl_Srh_Target" runat="server" CssClass="form-control"></asp:DropDownList>
                    </li>
                    <li>檔案分類：<asp:DropDownList ID="ddl_Srh_Type" runat="server" CssClass="form-control"></asp:DropDownList>
                    </li>
                    <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" CssClass="form-control" placeholder="檔名, 關聯品號"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Button ID="btn_ListSearch" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    </li>
                </ul>

            </div>
            <div class="table-responsive">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                    <LayoutTemplate>
                        <table class="List1 table" width="100%">
                            <tr class="tdHead">
                                <td>資料編號
                                </td>
                                <td>下載對象
                                </td>
                                <td>分類 / 檔名</td>
                                <td>最後更新</td>
                                <td>&nbsp;</td>
                            </tr>
                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="trItem" runat="server">
                            <td align="center"><%#Eval("SeqNo") %></td>
                            <td align="center"><%#Eval("TargetName") %></td>
                            <td align="left">
                                <span class="label label-warning"><%#Eval("FileTypeName") %></span>
                                <h5><strong><%#Eval("DisplayName") %></strong></h5>
                            </td>
                            <td align="center">
                                <table class="TableS1" width="95%">
                                    <tbody>
                                        <tr>
                                            <td class="TableS1TdHead">更新者</td>
                                            <td><%#Eval("MtName") %></td>
                                        </tr>
                                        <tr>
                                            <td class="TableS1TdHead">更新時間</td>
                                            <td><%#Eval("MtTime").ToString().ToDateString("yyyy-MM-dd HH:mm") %></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                            <td align="center">
                                <a href="<%=Application["WebUrl"] %>Ashx_FtpFileDownload.ashx?dwFolder=<%#UploadFolder %>&realFile=<%#Eval("FileName") %>&dwFileName=<%#Eval("DisplayName") %>" class="btn btn-success">&nbsp;<i class="fa fa-cloud-download"></i>&nbsp;</a>
                                <a href="myDW.aspx?Class=<%#HttpUtility.UrlEncode(Req_Class) %>&LangType=<%#HttpUtility.UrlEncode(Req_LangType) %>&DataID=<%#Eval("File_ID").ToString()%>" class="btn btn-primary">修改</a>
                                <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("File_ID") %>' />
                                <asp:HiddenField ID="hf_FileName" runat="server" Value='<%#Eval("FileName") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div style="padding: 30px 0;" class="text-center text-danger">
                            <h3>尚未新增檔案...</h3>
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
            <asp:Panel ID="pl_Page" runat="server" CssClass="PagesArea" Visible="false">
                <div class="PageControlCon">
                    <div class="PageControl">
                        <asp:Literal ID="lt_Page_Link" runat="server" EnableViewState="False"></asp:Literal>
                        <span class="PageSet">轉頁至
                    <asp:DropDownList ID="ddl_Page_List" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Page_List_SelectedIndexChanged">
                    </asp:DropDownList>
                            /
                    <asp:Literal ID="lt_TotalPage" runat="server" EnableViewState="False"></asp:Literal>
                            頁</span>
                    </div>
                    <div class="PageAccount">
                        <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                </div>
            </asp:Panel>
        </asp:PlaceHolder>
    </form>
</body>
</html>
