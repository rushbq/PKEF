<%@ Page Title="客戶商品對應-匯入" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="RefCopmg_Import.aspx.cs" Inherits="mySZBBC_Extend_RefCopmg_Import" %>

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
                    <div class="section">BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        客戶商品對應-匯入
                    </div>
                </div>
            </div>
            <div class="right menu">
                <%--<a href="<%=FuncPath() %>RefCopmg.aspx" class="item"><i class="undo icon"></i><span class="mobile hidden">返回</span></a>--%>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <!-- Section Start -->
            <div class="ui segments">
                <div class="ui green segment">
                    <h5 class="ui header">工具</h5>
                </div>
                <div class="ui small form segment">
                    <div class="fields">
                        <div class="four wide field">
                            <label>選擇商城</label>
                            <asp:DropDownList ID="ddl_Mall1" runat="server" CssClass="fluid">
                            </asp:DropDownList>
                        </div>
                        <div class="five wide required field">
                            <label>客戶(深圳)</label>
                            <div class="ui fluid search ac-Cust1">
                                <div class="ui right labeled input">
                                    <asp:TextBox ID="filter_Cust1" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                    <asp:Label ID="lb_Cust1" runat="server" CssClass="ui label" Text="輸入關鍵字,選擇項目"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="seven wide field">
                            <label>選擇Excel檔&nbsp;<a href="<%=fn_Params.RefUrl %>PKEF/Import_COPMG.xlsx" target="_blank">(範本下載)</a></label>
                            <div class="two fields">
                                <div class="field">
                                    <asp:FileUpload ID="fu_File1" runat="server" AllowMultiple="false" accept=".xlsx" />
                                </div>
                                <div class="field">
                                    <asp:Button ID="btn_Save1" runat="server" Text="開始上傳" OnClick="btn_Save1_Click" CssClass="ui green small button" />

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="ui orange segment">
                    <h5 class="ui header">科學玩具</h5>
                </div>
                <div class="ui small form segment">
                    <div class="fields">
                        <div class="four wide field">
                            <label>選擇商城</label>
                            <asp:DropDownList ID="ddl_Mall2" runat="server" CssClass="fluid">
                            </asp:DropDownList>
                        </div>
                        <div class="five wide required field">
                            <label>客戶(上海)</label>
                            <div class="ui fluid search ac-Cust2">
                                <div class="ui right labeled input">
                                    <asp:TextBox ID="filter_Cust2" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                    <asp:Label ID="lb_Cust2" runat="server" CssClass="ui label" Text="輸入關鍵字,選擇項目"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="seven wide field">
                            <label>選擇Excel檔&nbsp;<a href="<%=fn_Params.RefUrl %>PKEF/Import_COPMG.xlsx" target="_blank">(範本下載)</a></label>
                            <div class="two fields">
                                <div class="field">
                                    <asp:FileUpload ID="fu_File2" runat="server" AllowMultiple="false" accept=".xlsx" />
                                </div>
                                <div class="field">
                                    <asp:Button ID="btn_Save2" runat="server" Text="開始上傳" OnClick="btn_Save2_Click" CssClass="ui green small button" />

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Section End -->
            <div class="ui segments">
                <div class="ui blue segment">
                    <h5 class="ui header">重複資料檢查&nbsp;<small>(若有重複僅需保留一筆)</small></h5>
                </div>
                <div class="ui segment">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                        <LayoutTemplate>
                            <table class="ui celled selectable compact small table">
                                <thead>
                                    <tr>
                                        <th class="grey-bg lighten-3 collapsing">系統編號</th>
                                        <th class="grey-bg lighten-3">商城</th>
                                        <th class="grey-bg lighten-3">客戶代號</th>
                                        <th class="grey-bg lighten-3">寶工品號</th>
                                        <th class="grey-bg lighten-3">客戶品號</th>
                                        <th class="grey-bg lighten-3"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="center aligned"><%#Eval("Data_ID") %></td>
                                <td class="center aligned"><%#Eval("MallName") %></td>
                                <td class="center aligned"><%#Eval("CustID") %></td>
                                <td><%#Eval("ModelNo") %></td>
                                <td><%#Eval("CustModelNo") %></td>
                                <td class="center aligned collapsing">
                                    <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')"><i class="trash alternate icon"></i></asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>目前無重複資料</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
    <div class="ui info message">
        <div class="header">
            功能說明
        </div>
        <ul class="list">
            <li class="red-text text-darken-2">
                <h3>*** 對應表匯入 "千萬不可" 與BBC匯入同時進行，否則會有不可預期的後果!!!***</h3>
            </li>
            <li>匯入格式請下載範本。</li>
            <li>商城及客戶的欄位請謹慎填寫。</li>
            <li>匯入時會先將舊資料清空，然後再匯入新資料。</li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown list
            $('select').dropdown();


        });
    </script>

    <%-- Search UI Start --%>
    <script>
        /* 客戶SZ (一般查詢) */
        $('.ac-Cust1').search({
            minCharacters: 1,
            fields: {
                results: 'results',
                title: 'ID',
                description: 'Label'
            },
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                $("#MainContent_lb_Cust1").text(result.Label);
                $("#MainContent_filter_CustName1").val(result.Label);
            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=2&q={query}'
            }

        });

    /* 客戶SH (一般查詢) */
    $('.ac-Cust2').search({
        minCharacters: 1,
        fields: {
            results: 'results',
            title: 'ID',
            description: 'Label'
        },
        searchFields: [
            'title',
            'description'
        ]
        , onSelect: function (result, response) {
            $("#MainContent_lb_Cust2").text(result.Label);
            $("#MainContent_filter_CustName2").val(result.Label);
        }
        , apiSettings: {
            url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=3&q={query}'
        }

    });
    </script>

    <%-- Search UI End --%>
</asp:Content>

