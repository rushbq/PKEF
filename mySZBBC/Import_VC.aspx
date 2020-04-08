<%@ Page Title="深圳-工具BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Import_VC.aspx.cs" Inherits="mySZBBC_Import_VC" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">京東VC專用</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳BBC</a></li>
                        <li class="active"><a>京東VC上傳Excel</a></li>
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
                <p>
                    <asp:Literal ID="lt_Msg" runat="server"></asp:Literal></p>
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
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>注意事項</label>
                        <div>
                            <ul class="collection">
                                <li class="collection-item red-text"><i class="material-icons left">info</i>Excel匯入資料請務必放在「第一個工作表(Sheet)」</li>
                                <li class="collection-item"><i class="material-icons left">info</i>上傳後,系統會自動將所有客戶拆成單一客戶</li>
                                <li class="collection-item"><i class="material-icons left">info</i>後續的匯入流程不變,請至列表頁,將各客戶繼續匯入</li>
                                <li class="collection-item"><i class="material-icons left">info</i>若客戶未出現,請聯絡資訊部,並告知客戶名稱及代號</li>
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
                    <a class="btn-large waves-effect waves-light grey" href="<%=fn_Params.WebUrl %>/mySZBBC/ImportIndex.aspx">上一步，選擇模式<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click">開始上傳<i class="material-icons right">chevron_right</i></asp:LinkButton>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">

</asp:Content>

