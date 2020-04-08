<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Req_Process.aspx.cs" Inherits="Recording_Req_Process" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">資訊需求 - 主管核示</h5>
                    <ol class="breadcrumb">
                        <li><a href="#!">資訊需求</a></li>
                        <li class="active">主管核示</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <asp:PlaceHolder ID="ph_Message" runat="server">
            <div class="card-panel green darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>作業已完成，請關閉視窗</h4>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Error" runat="server" Visible="false">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>作業失敗，請重試</h4>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="row">
                    <div class="col s12 m6 l6">
                        <label>追蹤編號</label>
                        <div class="red-text text-darken-2 flow-text">
                            <b>
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal></b>
                        </div>
                    </div>
                    <div class="col s12 m6 l6">
                        <label>需求者</label>
                        <div class="green-text text-darken-2 flow-text">
                            <b>
                                <asp:Literal ID="lt_ReqWho" runat="server"></asp:Literal></b>
                        </div>
                    </div>
                </div>

                <div class="section row">
                    <div class="col s12">
                        <label>主旨</label>
                        <p>
                            <asp:Literal ID="lt_Help_Subject" runat="server"></asp:Literal>
                        </p>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>說明</label>
                        <p>
                            <asp:Literal ID="lt_Help_Content" runat="server"></asp:Literal>
                        </p>
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
                    <asp:LinkButton ID="lbtn_No" runat="server" CssClass="btn-large waves-effect waves-light grey" OnClick="lbtn_No_Click">不同意<i class="material-icons left">clear</i></asp:LinkButton>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Yes" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Yes_Click">同意<i class="material-icons left">done</i></asp:LinkButton>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

