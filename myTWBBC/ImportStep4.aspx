<%@ Page Title="台灣BBC | 完成轉入" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep4.aspx.cs" Inherits="myTWBBC_ImportStep4" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<%@ Import Namespace="PKLib_Method.Methods" %>
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
                    <div class="section">台灣BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step4.完成
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
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="4" />
            <!-- 資料 Start -->
            <div class="ui small form attached green segment">
                <div class="fields">
                    <div class="six wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="six wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                    <div class="four wide field">
                        <div class="ui middle aligned divided list">
                            <div class="item">
                                <div class="right floated content">
                                    <strong>
                                        <asp:Label ID="lb_DataType" runat="server"></asp:Label>
                                    </strong>
                                </div>
                                <div class="content">
                                    匯入類型
                                </div>
                            </div>
                            <div class="item">
                                <div class="right floated content">
                                    <strong>
                                        <asp:Label ID="lb_OrderType" runat="server"></asp:Label>
                                    </strong>
                                </div>
                                <div class="content">
                                    ERP單別
                                </div>
                            </div>
                            <div class="item">
                                <div class="right floated content">
                                    <asp:Label ID="lb_Currency" runat="server"></asp:Label>
                                </div>
                                <div class="content">
                                    幣別
                                </div>
                            </div>
                        </div>

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

