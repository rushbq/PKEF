<%@ Page Title="" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5" style="min-height: 550px;">
            <div class="ui segments">
                <div class="ui grey segment">
                    <h5 class="ui header">快速連結
                    </h5>
                </div>
                <div class="ui small form attached segment">
                    <a href="<%=fn_Params.WebUrl %>AirMIS/ITHelp_Edit.aspx" class="ui large blue icon button" target="_blank"><i class="pencil icon"></i>填寫資訊需求</a>
                    <a href="<%=fn_Params.WebUrl %>AirMIS/ITHelp_Search.aspx?who=<%=fn_Params.UserAccount %>" class="ui large green icon button" target="_blank"><i class="tasks icon"></i>追蹤我的需求</a>
                </div>
            </div>
            <div class="ui segments">
                <div class="ui grey segment">
                    <h5 class="ui header">BBC平台
                    </h5>
                </div>
                <div class="ui attached segment">
                    <div class="ui three cards">
                        <a class="blue card" href="<%=fn_Params.WebUrl %>myTWBBC_Mall/ImportList.aspx">
                            <div class="content">
                                <div class="header">台灣電商BBC</div>
                                <div class="meta">
                                    <span class="category">資料庫:台灣</span>
                                </div>
                                <div class="description">
                                    <p>台灣電商訂單</p>
                                </div>
                            </div>
                        </a>
                        <a class="violet card" href="<%=fn_Params.WebUrl %>myTWBBC/ImportList.aspx">
                            <div class="content">
                                <div class="header">台灣BBC</div>
                                <div class="meta">
                                    <span class="category">資料庫:自訂</span>
                                </div>
                                <div class="description">
                                    <p>外銷經銷訂單</p>
                                    <p>可指定台灣/上海資料庫,或是依產品出貨地.</p>
                                </div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="ui attached segment">
                    <div class="ui three cards">
                        <a class="red card" href="<%=fn_Params.WebUrl %>mySHBBC/ImportList.aspx">
                            <div class="content">
                                <div class="header">上海工具BBC</div>
                                <div class="meta">
                                    <span class="category">資料庫:上海</span>
                                </div>
                                <div class="description">
                                    <p>上海電商訂單</p>
                                </div>
                            </div>
                        </a>
                        <a class="orange card" href="<%=fn_Params.WebUrl %>mySZBBC_Toy/ImportList.aspx">
                            <div class="content">
                                <div class="header">玩具BBC</div>
                                <div class="meta">
                                    <span class="category">資料庫:上海</span>
                                </div>
                                <div class="description">
                                    <p>玩具訂單</p>
                                </div>
                            </div>
                        </a>
                    </div>


                </div>


            </div>

        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

