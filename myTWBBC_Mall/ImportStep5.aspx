<%@ Page Title="台灣電商BBC | Step5" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep5.aspx.cs" Inherits="myTWBBC_Mall_ImportStep5" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">業務行銷</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">台灣電商BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step5.完成
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="ui negative message">
                    <div class="header">
                        Oops...發生了一點小問題
                    </div>
                    <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                    <%--<a href="<%:FuncPath() %>/View.aspx?id=<%=Req_DataID %>">點我看詳細</a>--%>
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="5" />
            <!-- 資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <div class="fields">
                    <div class="five wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="five wide field">
                        <label>商城</label>
                        <asp:Label ID="lb_Mall" runat="server" CssClass="ui brown basic large label"></asp:Label>
                    </div>
                    <div class="six wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                </div>
                <div class="field">
                    <div class="ui placeholder segment">
                        <div class="ui icon header">
                            <i class="thumbs up icon"></i>
                            資料已轉入EDI排程，接下來你可以...
                        </div>
                        <div class="inline">
                            <a class="ui button" href="<%=Page_SearchUrl %>">回列表頁</a>
                            <a class="ui blue button" href="<%=FuncPath() %>/ImportStep1.aspx">新增匯入</a>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

