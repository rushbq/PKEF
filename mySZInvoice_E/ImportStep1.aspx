<%@ Page Title="深圳-電子發票 | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep1.aspx.cs" Inherits="mySZInvoiceE_ImportStep1" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step1 - 檔案上傳</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-電子發票</a></li>
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
        <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>Oops...發生了一點小問題</h4>                
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
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
         <div class="row">
            <div class="col s12 m12 l12">
                <div class="card-panel">
                    <div class="section row">
                        <div class="col s12">
                            <label>匯入範本</label>
                            <div class="collection">
                                <a href="<%=Application["RefUrl"] %>PKEF/SZInvoice_Sample/Sample.xlsx" class="collection-item" target="_blank">範本下載<i class="material-icons right">cloud_download</i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a class="btn-large waves-effect waves-light grey" href="<%=Application["WebUrl"] %>/mySZInvoice_E/ImportList.aspx">返回列表<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click">下一步，選擇工作表<i class="material-icons right">chevron_right</i></asp:LinkButton>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">

</asp:Content>

