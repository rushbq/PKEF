<%@ Page Title="上海會計 | 電子發票匯入 | Step1" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep1.aspx.cs" Inherits="mySHInvoiceE_ImportStep1" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step1.檔案上傳</h5>
                    <ol class="breadcrumb">
                        <li><a>上海會計</a></li>
                        <li><a>電子發票匯入</a></li>
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
                <div class="section row">
                    <div class="col s12">
                        <label>功能說明</label>
                        <div>
                            <ul class="collection">
                                <li><a href="<%=Application["RefUrl"] %>PKEF/SZInvoice_Sample/Sample.xlsx" class="collection-item" target="_blank">Excel匯入範本下載<i class="material-icons right">cloud_download</i></a></li>
                                <li class="collection-item"><i class="material-icons left">info</i>此功能為回寫電商發票資料</li>
                                <li class="collection-item red-text text-darken-1"><i class="material-icons left">info</i>匯入Excel後，對應「訂單:客戶單號」，將發票號碼、發票日回填至結帳單</li>
                            </ul>
                        </div>
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
                    <a class="btn-large waves-effect waves-light grey" href="<%=Application["WebUrl"] %>/mySHInvoice_E/ImportList.aspx">返回列表<i class="material-icons left">arrow_back</i></a>
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

