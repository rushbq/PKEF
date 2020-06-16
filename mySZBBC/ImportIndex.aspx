﻿<%@ Page Title="深圳-工具BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportIndex.aspx.cs" Inherits="mySZBBC_ImportIndex" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">選擇匯入模式</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳BBC</a></li>
                        <li><a>匯入Excel</a></li>
                        <li class="active">選擇匯入模式</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <div class="row">
            <div class="col s12 m12 l12">
                <div class="card-panel">
                    <div>
                        <asp:LinkButton ID="lbtn_link1" runat="server" CssClass="btn-large waves-effect waves-light green" OnClick="lbtn_link1_Click">未出貨訂單<i class="material-icons right">chevron_right</i></asp:LinkButton>

                        <%--<a href="<%=Application["WebUrl"] %>mySZBBC/ImportStep1.aspx?ts=<%=TraceID %>&type=1" class="btn-large waves-effect waves-light green">未出貨訂單<i class="material-icons right">chevron_right</i></a>
                        <a href="<%=Application["WebUrl"] %>mySZBBC/ImportStep1.aspx?ts=<%=TraceID %>&type=3" class="btn-large waves-effect waves-light green darken-3">已出貨訂單<i class="material-icons right">chevron_right</i></a>--%>
                        <a href="<%=Application["WebUrl"] %>mySZBBC/Import_VC.aspx" class="btn-large waves-effect waves-light blue darken-2">京東VC匯入<i class="material-icons right">chevron_right</i></a>
                    </div>
                    <div class="section row">
                        <div class="col s12">
                            <label>匯入範本</label>
                            <div class="collection">
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/JD-POP.xlsx" class="collection-item" target="_blank">京東POP<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/TMall.xlsx" class="collection-item" target="_blank">天貓<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/JD-VC.xlsx" class="collection-item" target="_blank">京東VC<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/eService.xlsx" class="collection-item" target="_blank">eService<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/VIPS.xlsx" class="collection-item" target="_blank">唯品會<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/JD-Factory.xlsx" class="collection-item" target="_blank">京東廠送<i class="material-icons right">cloud_download</i></a>
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/Public.xlsx" class="collection-item" target="_blank">通用版<i class="material-icons right">cloud_download</i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col s12 m12 l12">
                <div class="card-panel">
                    <div>
                        <asp:LinkButton ID="lbtn_link2" runat="server" CssClass="btn-large waves-effect waves-light deep-orange darken-1" OnClick="lbtn_link2_Click">退貨單匯入<i class="material-icons right">chevron_right</i></asp:LinkButton>
                    </div>
                    <div class="section row">
                        <div class="col s12">
                            <label>匯入範本</label>
                            <div class="collection">
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/RT-JD-VC.xlsx" class="collection-item" target="_blank">京東VC<i class="material-icons right">cloud_download</i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

