<%@ Page Title="玩具BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep1.aspx.cs" Inherits="mySZBBC_ImportStep1" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="blue lighten-5">
        <div class="container">
            <div class="row">
                <div class="col s12">
                    <h5 class="breadcrumbs-title">Step1 - 檔案上傳</h5>
                    <ol class="breadcrumb">
                        <li><a>玩具BBC</a></li>
                        <li><a>匯入Excel</a></li>
                        <li class="active">Step1</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <asp:PlaceHolder ID="ph_Message" runat="server">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>Oops...發生了一點小問題</h4>
                <p><a class="btn waves-effect waves-light grey darken-1" href="<%=Application["WebUrl"] %>mySZBBC_Toy/ImportIndex.aspx">請點此返回, 再重新操作一次.</a></p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="row">
                    <div class="col s12">
                        <label>目前選擇模式</label>
                        <div class="red-text text-darken-2 flow-text"><b><%=fn_Desc.BBC.Get_ModeName(Req_Type) %></b></div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>選擇商城&nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_Mall" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="ddl_Mall" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator></label>
                        <asp:DropDownList ID="ddl_Mall" runat="server" CssClass="browser-default">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>選擇客戶&nbsp;<asp:RequiredFieldValidator ID="rfv_Cust_ID_Val" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="Cust_ID_Val" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator></label>
                        <asp:TextBox ID="Cust_Name" runat="server" CssClass="AC-Customer" data-target="MainContent_Cust_ID_Val" placeholder="輸入客戶關鍵字"></asp:TextBox>
                        <asp:TextBox ID="Cust_ID_Val" runat="server" Style="display: none"></asp:TextBox>
                        <asp:TextBox ID="Sales_ID_Val" runat="server" Style="display: none"></asp:TextBox>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>選擇檔案&nbsp;<asp:RequiredFieldValidator ID="rfv_file_Upload" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="file_Upload" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator></label>
                        <div class="file-field input-field">
                            <div class="btn">
                                <span>File</span>
                                <asp:FileUpload ID="file_Upload" runat="server" AllowMultiple="false" accept=".xls,.xlsx" />
                            </div>
                            <div class="file-path-wrapper">
                                <input class="file-path validate" type="text" placeholder="請選擇要匯入的檔案" />
                            </div>
                        </div>
                    </div>
                    <div class="red-text text-darken-1">
                        <asp:Literal ID="lt_UploadMessage" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a class="btn-large waves-effect waves-light grey" href="<%=Application["WebUrl"] %>/mySZBBC_Toy/ImportIndex.aspx">上一步，選擇模式<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click">下一步，選擇工作表<i class="material-icons right">chevron_right</i></asp:LinkButton>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.js"></script>
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.js"></script>
    <%-- Catcomplete Start --%>
    <script>
        /* Autocomplete 客戶 */
        $(".AC-Customer").autocomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/GetData_Customer.ashx",
                    data: {
                        keyword: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    id: item.ID,
                                    label: item.Label + ' (' + item.ID + ')',
                                    sales: item.SalesID
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                //目前欄位
                $(this).val(ui.item.value);

                //實際欄位-儲存值
                var targetID = $(this).attr("data-target");
                $("#" + targetID).val(ui.item.id);

                //其他欄位-業務人員ID
                $("#MainContent_Sales_ID_Val").val(ui.item.sales);

                event.preventDefault();
            }
        });
    </script>
    <%-- Catcomplete End --%>
</asp:Content>

