<%@ Page Title="客戶品號重複檢查 (ERP)" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="CheckCopmg.aspx.cs" Inherits="myBBC_Extend_CheckCopmg" %>

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
                        客戶品號重複檢查 (ERP)
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
                    <h5 class="ui header">上海資料庫</h5>
                </div>
                <div class="ui small form segment">
                    <div class="sixteen wide field">
                        <label>客戶(上海)</label>
                        <div class="two fields">
                            <div class="field">
                                <div class="ui fluid search ac-Cust1">
                                    <div class="ui right labeled input">
                                        <asp:TextBox ID="filter_Cust1" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                        <asp:Label ID="lb_Cust1" runat="server" CssClass="ui label" Text="輸入關鍵字,選擇項目"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Button ID="btn_Check1" runat="server" Text="開始檢查" CssClass="ui green small button" OnClick="btn_Check1_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="ui orange segment" style="display:none;">
                    <h5 class="ui header">台灣資料庫</h5>
                </div>
                <div class="ui small form segment" style="display:none;">
                    <div class="sixteen wide field">
                        <label>客戶(台灣)</label>
                        <div class="two fields">
                            <div class="field">
                                <div class="ui fluid search ac-Cust2">
                                    <div class="ui right labeled input">
                                        <asp:TextBox ID="filter_Cust2" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                        <asp:Label ID="lb_Cust2" runat="server" CssClass="ui label" Text="輸入關鍵字,選擇項目"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="field">
                                <asp:Button ID="btn_Check2" runat="server" Text="開始檢查" CssClass="ui green small button" OnClick="btn_Check2_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Section End -->
            <div class="ui segments">
                <div class="ui blue segment">
                    <h5 class="ui header">
                        查詢結果&nbsp;<small><asp:Literal ID="lt_CustID" runat="server"></asp:Literal></small>
                        &nbsp;<small class="grey-text text-darken-2">(資料來源：ERP客戶品號資料)</small>
                    </h5>
                </div>
                <div class="ui segment">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="ui celled selectable compact small table">
                                <thead>
                                    <tr>
                                        <th class="grey-bg lighten-3">寶工品號</th>
                                        <th class="grey-bg lighten-3">客戶品號</th>
                                        <th class="grey-bg lighten-3">客戶品名</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("PKModel") %></td>
                                <td><%#Eval("CustModel") %></td>
                                <td><%#Eval("ModelName") %></td>
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

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">

    <%-- Search UI Start --%>
    <script>
        /* 客戶SH (一般查詢) */
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
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=3&q={query}'
            }

        });

    /* 客戶TW (一般查詢) */
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
            url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=1&q={query}'
        }

    });
    </script>

    <%-- Search UI End --%>
</asp:Content>

