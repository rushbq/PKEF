<%@ Page Title="上海BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportIndex.aspx.cs" Inherits="mySHBBC_ImportIndex" %>

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
                        <li><a>上海BBC</a></li>
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

                        <a href="<%=fn_Params.WebUrl %>mySHBBC/Import_VC.aspx" class="btn-large waves-effect waves-light blue darken-2">京東VC匯入<i class="material-icons right">chevron_right</i></a>
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
                <!-- 退貨 -->
                <div class="card-panel">
                    <div class="section row">
                        <div class="col s8">
                            <asp:LinkButton ID="lbtn_link2" runat="server" CssClass="btn-large waves-effect waves-light deep-orange darken-1" OnClick="lbtn_link2_Click">退貨單匯入<i class="material-icons right">chevron_right</i></asp:LinkButton>
                            <a href="<%=fn_Params.WebUrl %>mySHBBC/RebImport_VC.aspx" class="btn-large waves-effect waves-light brown darken-2">京東VC批量退貨<i class="material-icons right">chevron_right</i></a>
                        </div>
                        <div class="col s4 right-align">
                            <a href="#infoModal" class="btn-large waves-effect waves-light light-blue darken-1 modal-trigger"><i class="material-icons right">live_help</i>銷退備註代號</a>
                        </div>
                    </div>

                    <div class="section row">
                        <div class="col s12">
                            <label>匯入範本</label>
                            <div class="collection">
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/RT-JD-VC.xlsx" class="collection-item" target="_blank">京東VC<i class="material-icons right">cloud_download</i></a>                                
                                <a href="<%=Application["RefUrl"] %>PKEF/SZBBC_Sample/VC-Batch-Reback.xlsx" class="collection-item" target="_blank">VC批量<i class="material-icons right">cloud_download</i></a>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Modal Structure -->
                <div id="infoModal" class="modal modal-fixed-footer">
                    <div class="modal-content">
                        <h4>銷退備註代號對應表</h4>
                        <div>
                            <asp:ListView ID="lv_RemarkList" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table class="ui celled striped table">
                                        <thead>
                                            <tr>
                                                <th>代號</th>
                                                <th>原因</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><b>
                                            <%#Eval("ID") %></b>
                                        </td>
                                        <td>
                                            <%#Eval("Label") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <a href="#!" class="modal-close waves-effect waves-green btn-flat">Close</a>
                    </div>
                </div>

            </div>
        </div>


    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(document).ready(function () {
            $('.modal').modal();
        });
    </script>
</asp:Content>

