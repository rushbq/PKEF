<%@ Page Title="網站功能點擊記錄" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="MenuClickSearch.aspx.cs" Inherits="AirMIS_MenuClickSearch" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">資訊服務</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section red-text text-darken-2">
                        網站功能點擊記錄
                    </div>
                </div>
            </div>
            <div class="right menu">
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="fields">
                    <div class="two wide field">
                        <label>選擇年份</label>
                        <asp:DropDownList ID="ddl_Year" runat="server" CssClass="fluid">
                        </asp:DropDownList>
                    </div>
                    <div class="three wide field">
                        <label>年月(開始)</label>
                        <div class="two fields">
                            <div class="field">
                                <asp:DropDownList ID="ddl_SM" runat="server" CssClass="fluid">
                                </asp:DropDownList>
                            </div>
                            <div class="field">
                                <asp:DropDownList ID="ddl_SD" runat="server" CssClass="fluid">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="three wide field">
                        <label>年月(結束)</label>
                        <div class="two fields">
                            <div class="field">
                                <asp:DropDownList ID="ddl_EM" runat="server" CssClass="fluid">
                                </asp:DropDownList>
                            </div>
                            <div class="field">
                                <asp:DropDownList ID="ddl_ED" runat="server" CssClass="fluid">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="three wide field">
                        <label>選擇網站</label>
                        <asp:DropDownList ID="ddl_Website" runat="server" CssClass="fluid">
                            <asp:ListItem Value="">-- 請選擇 --</asp:ListItem>
                            <asp:ListItem Value="1">產品中心 (ProductCenter)</asp:ListItem>
                            <asp:ListItem Value="2">內部管理網站 (PKHome)</asp:ListItem>
                            <asp:ListItem Value="3">內部系統整合 (PKEF)</asp:ListItem>
                            <asp:ListItem Value="4">報表中心 (PKReport)</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="five wide field">
                        <label>&nbsp;</label>
                        <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>查詢</button>
                        <asp:Button ID="btn_Search" runat="server" Text="Button" OnClick="btn_Search_Click" Style="display: none" />
                    </div>
                </div>
            </div>

        </div>
        <!-- Search End -->
        <!-- Content Start -->
        <div class="ui green attached segment">
            <div class="ui secondary pointing menu">
                <a class="item <%if (Req_Tab.Equals("1")) Response.Write("active"); %>" href="<%=thisPage %>?tab=1">樹狀模式</a>
                <a class="item <%if (Req_Tab.Equals("2")) Response.Write("active"); %>" href="<%=thisPage %>?tab=2">列表模式</a>
            </div>
            <div class="ui small form">
                <div class="fields">
                    <div class="sixteen wide field">
                        <asp:PlaceHolder ID="ph_tab1" runat="server">
                            <!-- zTree js -->
                            <div id="menuList" class="ztree"></div>
                            <asp:PlaceHolder ID="ph_tab1emptyData" runat="server">
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="search icon"></i>請先設定條件，開始查詢
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_tab2" runat="server">
                            <!-- List Table -->
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="ui celled selectable large table">
                                        <thead>
                                            <tr>
                                                <th class="grey-bg lighten-3 collapsing">系統序號</th>
                                                <th class="grey-bg lighten-3 center aligned">第一層</th>
                                                <th class="grey-bg lighten-3 center aligned">第二層</th>
                                                <th class="grey-bg lighten-3 center aligned">第三層</th>
                                                <th class="grey-bg lighten-3 center aligned">第四層</th>
                                                <th class="grey-bg lighten-3 center aligned collapsing">點擊數</th>
                                                <th class="grey-bg lighten-3"></th>
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
                                        <td class="center aligned collapsing">
                                            <%#Eval("MenuID") %>
                                        </td>
                                        <td class="center aligned">
                                            <%#Eval("Lv1Name") %>
                                        </td>
                                        <td class="center aligned">
                                            <%#Eval("Lv2Name") %>
                                        </td>
                                        <td class="center aligned">
                                            <%#Eval("Lv3Name") %>
                                        </td>
                                        <td class="center aligned">
                                            <%#Eval("Lv4Name") %>
                                        </td>
                                        <td class="center aligned collapsing">
                                            <b><%#Eval("ClickCnt") %></b>
                                        </td>
                                        <td class="center aligned collapsing">
                                            <%--<a class="ui small teal basic icon button" href="EqEdit.aspx?dbs=<%:Req_DBS %>&id=<%#Eval("Data_ID") %>" title="編輯">
                                                <i class="pencil icon"></i>
                                            </a>--%>
                                            <asp:Literal ID="lt_RemarkUrl" runat="server"></asp:Literal>
                                            <input type="hidden" id="msgDetail_<%#Eval("MenuID") %>" value="<%#Eval("Remark") %>">
                                        </td>
                                        <td class="center aligned collapsing">
                                            <asp:Literal ID="lt_ClickWho" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>

                            <asp:PlaceHolder ID="ph_tab2emptyData" runat="server">
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="search icon"></i>請先設定條件，開始查詢
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
        <!-- Content End -->
        <!-- Msg Modal Start -->
        <div id="msgPage" class="ui modal">
            <div class="header">
                <span id="msgTitle" class="green-text text-darken-2"></span>說明
            </div>
            <div class="content">
                <div class="ui message">
                    <p id="msgCont"></p>
                </div>
            </div>
            <div class="actions">
                <div class="ui cancel button">
                    關閉視窗
                </div>
            </div>
        </div>
        <!-- Detail Modal End -->
        <!-- Msg Modal Start -->
        <div id="dtPage" class="ui modal">
            <div class="header">
                <span id="dtTitle" class="green-text text-darken-2"></span>名單
            </div>
            <div class="scrolling content">
                <table class="ui striped celled table">
                    <thead>
                        <tr class="center aligned">
                            <th>次數</th>
                            <th>誰</th>
                        </tr>
                    </thead>
                    <tbody id="dtCont"></tbody>
                </table>
            </div>
            <div class="actions">
                <div class="ui cancel button">
                    關閉視窗
                </div>
            </div>
        </div>
        <!-- Detail Modal End -->
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            $(".myContentBody").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#doSearch").trigger("click");
                    //避免觸發submit
                    e.preventDefault();
                }
            });

            //[搜尋][查詢鈕] - 觸發查詢
            $("#doSearch").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });

            //init dropdown list
            $('select').dropdown();

        });
    </script>

    <!-- 樹狀選單 js -->
    <asp:PlaceHolder ID="ph_treeJS" runat="server" Visible="false">
        <link rel="stylesheet" href="<%=fn_Params.WebUrl %>js/zTree/css/style.min.css" />
        <script src="<%=fn_Params.WebUrl %>js/zTree/fixed/jquery.ztree.core-3.5.min.js"></script>
        <script>
            //--- zTree 設定 Start ---
            var setting = {
                view: {
                    fontCss: getFont, //觸發css
                    nameIsHTML: true, //允許Html
                    addDiyDom: addDiyDom, //自訂dom
                    dblClickExpand: false   //已使用onclick展開,故將雙擊展開關閉                    
                },
                callback: {
                    onClick: MMonClick
                },
                check: {
                    enable: false
                },
                data: {
                    simpleData: {
                        enable: true
                    }
                }
            };

            //載入自訂字型樣式
            function getFont(treeId, node) {
                return node.font ? node.font : {};
            }

            //Event - onClick
            function MMonClick(e, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj(treeId);

                //展開/收合選單
                zTree.expandNode(treeNode);
            }

            //自訂DOM
            function addDiyDom(treeId, treeNode) {
                //Get tree object
                var aObj = $("#" + treeNode.tId + "_a");
                //Get values
                var _id = treeNode.id;
                var _remark = treeNode.remark;
                var _label = treeNode.label;

                //點擊數label
                var _cnt = treeNode.cnt;
                if (_cnt > 0) {
                    var infoStr = "<span class='ui small red label'>" + treeNode.cnt + "</span>";
                    infoStr += '<a href="#!" class="doShowDT green-text text-darken-1" data-id="' + _id + '" data-title="' + _label + '">(名單)</a>'

                    aObj.after(infoStr);
                }

                //備註modal
                var msg = '<input type="hidden" id="msgDetail_' + _id + '" value="' + _remark + '">';
                var msgUrl = '';
                if (_remark != "") {
                    msgUrl = '<a href="#!" class="doShowMsg blue-text text-darken-1" data-id="' + _id + '" data-title="' + _label + '">(說明)</a>'
                    + msg;

                    aObj.after(msgUrl);
                }
            }
            //--- zTree 設定 End ---
        </script>
        <script>
            $(function () {
                /*
                    取得MenuList
                    ref:http://api.jquery.com/jQuery.post/
                */

                var jqxhr = $.post("<%=fn_Params.WebUrl%>AirMIS/GetMenuList.ashx", {
                    db: '<%=Req_WebID%>',
                    y: '<%=Req_Year%>',
                    sm: '<%=Req_SM%>',
                    sd: '<%=Req_SD%>',
                    em: '<%=Req_EM%>',
                    ed: '<%=Req_ED%>'
                })
                  .done(function (data) {
                      //載入選單
                      $.fn.zTree.init($("#menuList"), setting, data)

                      //auto expandAll
                      var treeObj = $.fn.zTree.getZTreeObj("menuList");
                      treeObj.expandAll(true);

                      //說明modal
                      $(".doShowMsg").on("click", function () {
                          //取資料
                          var name = $(this).attr("data-title"); //title
                          var id = $(this).attr("data-id"); //id
                          var msg = $("#msgDetail_" + id).val(); //get hidden field value

                          //填入值
                          $("#msgTitle").text(name);
                          $("#msgCont").html(msg);

                          //顯示modal
                          $('#msgPage').modal('show');
                      });

                      //名單modal
                      $(".doShowDT").on("click", function () {
                          //取資料
                          var name = $(this).attr("data-title"); //title
                          var id = $(this).attr("data-id"); //id

                          //填入值
                          $("#dtTitle").text(name);
                          //$("#dtCont").html(msg);

                          //load html
                          var url = '<%=fn_Params.WebUrl%>' + "AirMIS/GetMenuDetail.ashx?db=<%=Req_WebID%>&y=<%=Req_Year%>&id=" + id;
                          var datablock = $("#dtCont");
                          datablock.empty();
                          datablock.load(url);

                          //顯示modal
                          $('#dtPage').modal('show');
                      });
                  })
                  .fail(function () {
                      alert("選單載入失敗");
                  });

            });
        </script>
    </asp:PlaceHolder>


    <!-- 列表模式 js -->
    <asp:PlaceHolder ID="ph_tableJS" runat="server" Visible="false">
        <script>
            $(function () {
                //說明modal
                $(".doShowMsg").on("click", function () {
                    //取資料
                    var name = $(this).attr("data-title"); //title
                    var id = $(this).attr("data-id"); //id
                    var msg = $("#msgDetail_" + id).val(); //get hidden field value

                    //填入值
                    $("#msgTitle").text(name);
                    $("#msgCont").html(msg);

                    //顯示modal
                    $('#msgPage').modal('show');
                });

                //名單modal
                $(".doShowDT").on("click", function () {
                    //取資料
                    var name = $(this).attr("data-title"); //title
                    var id = $(this).attr("data-id"); //id

                    //填入值
                    $("#dtTitle").text(name);
                    //$("#dtCont").html(msg);

                    //load html
                    var url = '<%=fn_Params.WebUrl%>' + "AirMIS/GetMenuDetail.ashx?db=<%=Req_WebID%>&y=<%=Req_Year%>&id=" + id;
                    var datablock = $("#dtCont");
                    datablock.empty();
                    datablock.load(url);

                    //顯示modal
                    $('#dtPage').modal('show');
                });
            });
        </script>
    </asp:PlaceHolder>

</asp:Content>

