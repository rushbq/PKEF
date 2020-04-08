<%@ Page Title="深圳-工具BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep4.aspx.cs" Inherits="mySZBBC_ImportStep4" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step4 - EDI匯入</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳BBC</a></li>
                        <li><a>匯入Excel</a></li>
                        <li class="active">Step4</li>
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
                <p>若持續看到此訊息, 請回報錯誤發生的 <strong class="flow-text">詳細狀況</strong> 及 <strong class="flow-text">追蹤編號</strong>。</p>
                <p><a class="btn waves-effect waves-light grey darken-1" href="<%=Application["WebUrl"] %>mySZBBC/ImportLog.aspx?dataID=<%=Req_DataID %>#log">或點此查看記錄</a></p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel purple darken-3 white-text">
                <h4><i class="material-icons right">error_outline</i>即將完成匯入</h4>
            </div>
            <div class="card-panel">
                <div class="row">
                    <div class="col s12 grey lighten-5">
                        <i class="material-icons">flag</i>&nbsp;
                        目前匯入模式為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_TypeName" runat="server"></asp:Literal></b></span>
                        , 商城為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_MallName" runat="server"></asp:Literal></b></span>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s6">
                        <label>追蹤編號</label>
                        <div class="red-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal></strong>
                        </div>
                    </div>
                    <div class="col s6">
                        <label>客戶</label>
                        <div class="green-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_CustName" runat="server"></asp:Literal></strong>
                        </div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>注意事項</label>
                        <div>
                            <ul class="collection">
                                <li class="collection-item"><i class="material-icons left">info</i>按「下一步，完成匯入」後，資料將轉入EDI排程。</li>
                                <li class="collection-item"><i class="material-icons left">info</i>轉入排程後，將無法返回修改。</li>
                                <li class="collection-item"><i class="material-icons left">info</i>庫存判斷：B倉->A倉，兩者皆不足則轉訂單。</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Hidden Field -->
            <asp:HiddenField ID="hf_MallID" runat="server" />
            <asp:HiddenField ID="hf_Type" runat="server" />
            <asp:HiddenField ID="hf_TraceID" runat="server" />
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a class="btn-large waves-effect waves-light grey" href="<%=Application["WebUrl"] %>mySZBBC/ImportStep3.aspx?dataID=<%=Req_DataID %>">上一步，確認資料<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，完成匯入<i class="material-icons right">chevron_right</i></asp:LinkButton>

                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="Next" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

