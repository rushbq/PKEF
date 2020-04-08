<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IT_HelpEdit.aspx.cs" Inherits="IT_HelpEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <%-- 多筆上傳 Start --%>
    <script src="../js/multiFile/jquery.MultiFile.pack.js" type="text/javascript"></script>
    <%-- 多筆上傳 End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //快速連結, 滑至指定目標
            $(".scrollme").click(function () {
                //取得元素
                var _thisID = $(this).attr("href");

                //滑動至指定ID
                $('html, body').animate({
                    scrollTop: $(_thisID).offset().top
                }, 600);
            });

            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
            });

            /* 日期選擇器 */
            $("#tb_Reply_Date").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1
            });

            //多筆上傳
            $('#fu_Pic').MultiFile({
                STRING: {
                    remove: '<img src="../images/trashcan.png" alt="x" border="0" width="14" />' //移除圖示
                },
                accept: '<%=FileExtLimit %>' //副檔名限制
            });

            //Save
            $("#doSave").click(function () {
                var flag = $("#hf_flag").val();
                //check
                if (!Page_ClientValidate("Add")) {
                    return false;
                }

                //tree checkbox
                if (flag == "Add") {
                    getCbValue('myTree', 'tb_Emps');
                }

                //hide button
                $(this).hide();

                //trigger click
                $("#btn_Save").trigger("click");
            });

        });

        //返回列表
        function goToList() {
            location.href = '<%=Session["BackListUrl"] %>';
        }

    </script>
    <%-- 連動式選單 Start --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //部門選單變動時觸發事件
            $('select#ddl_Dept').change(function () {
                var GetVal = $('#ddl_Dept option:selected').val();
                //取得部門人員
                GetEmployees(GetVal);

            });

            //人員選單變動時觸發事件
            $('select#ddl_Employees').change(function () {
                var GetVal = $('#ddl_Employees option:selected').val();
                //填入選擇的人員
                $("#tb_EmpValue").val(GetVal);

            });

            //若部門有帶預設值, 則自動觸發事件
            $('select#ddl_Dept').trigger("change");
        });

        /* 取得部門人員 - 連動選單 Start */
        function GetEmployees(DeptId) {
            var flag = $("#hf_flag").val();

            //宣告 - 取得物件,人員
            var myMenu = $('select#ddl_Employees');
            myMenu.empty();
            myMenu.append($('<option></option>').val('').text('loading.....'));

            //判斷部門編號是否空白
            if (DeptId.length == 0) {
                SetEmployeeMenuEmpty(myMenu);
                return false;
            }

            //這段必須加入, 不然會有No Transport的錯誤
            jQuery.support.cors = true;
            //API網址
            var uri = 'https://api.prokits.com.tw/api/employees/?deptid=' + DeptId;

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();

                    //加入選項
                    myMenu.append($('<option></option>').val('').text('-- 請選擇 --'));
                    $.each(data, function (key, item) {
                        myMenu.append($('<option></option>').val(item.UserId).text('(' + item.UserId + ') ' + item.UserName))
                    });

                    //判斷目前為新增或修改
                    if (flag.toUpperCase() == "ADD") {
                        //設定預設值
                        myMenu.val("<%=Session["Login_UserID"]%>");
                        $('select#ddl_Employees').trigger("change");
                    } else {
                        //若為修改, 則帶入資料
                        var getVal = $("#tb_EmpValue").val();
                        myMenu.val(getVal);
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    alert("無法取得人員選單\n\r" + err);
                });
            }

            //重設選單
            function SetEmployeeMenuEmpty(menuID) {
                //清空選項
                menuID.empty();

                //加入選項
                menuID.append($('<option></option>').val('').text('-- 請先選擇部門 --'));
            }
            /* 取得部門人員 - 連動選單 End */
    </script>
    <%-- 連動式選單 End --%>

    <%-- zTree Start --%>
    <link href="../js/zTree/css/zTreeStyle.css" rel="stylesheet" />
    <script src="../js/zTree/jquery.ztree.core-3.5.min.js"></script>
    <script src="../js/zTree/jquery.ztree.excheck-3.5.min.js"></script>
    <script>
        //zTree 設定
        var setting = {
            view: {
                dblClickExpand: false
            },
            callback: {
                onClick: MMonClick
            },
            check: {
                enable: true
            },
            data: {
                simpleData: {
                    enable: true
                }
            }
        };

        //Event - onClick
        function MMonClick(e, treeId, treeNode) {
            var zTree = $.fn.zTree.getZTreeObj("myTree");
            zTree.expandNode(treeNode);
        }

        //宣告節點
        var zNodes;

        //取得資料
        function getUserList() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Ajax_Data/Json_UserList.aspx",
                data: {
                    block: 'Y'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree"), setting, zNodes);
        }

        // 所有節點的收合(true = 展開, false = 折疊)
        function expandAll(objbool) {
            var treeObj = $.fn.zTree.getZTreeObj("myTree");
            treeObj.expandAll(objbool);
        }

        /* 取值(zTree名稱, 要放值的欄位名) */
        function getCbValue(eleName, valName) {
            var treeObj = $.fn.zTree.getZTreeObj(eleName);
            var nodes = treeObj.getCheckedNodes(true);
            var ids = "";
            for (var i = 0; i < nodes.length; i++) {
                //只取開頭為'v_'的值
                var myval = nodes[i].id;

                if (myval.substring(0, 2) == "v_") {
                    //字串組合, 加入分隔符號("||")
                    if (ids != "") {
                        ids += "||"
                    }

                    //取得id值
                    ids += myval.replace("v_", "");
                }
            }


            //輸出組合完畢的字串值
            document.getElementById(valName).value = ids;
            return true;
        }

        //Load
        $(document).ready(function () {
            getUserList();


            //顯示所有節點
            $('#showAll').click(function () {
                expandAll(true);
            });

            //隱藏所有節點
            $('#hideAll').click(function () {
                expandAll(false);
            });

        });


    </script>
    <%-- zTree End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>需求記錄表</a>&gt;<span>需求登記</span>
        </div>
        <div class="h2Head">
            <h2>需求登記</h2>
        </div>
        <div class="SysTab">
            <ul>
                <li class="TabAc"><a href="<%=PageUrl %>">資料設定</a></li>
                <li><a href="#notice" class="scrollme"><span class="B styleRed">功能說明</span></a></li>
            </ul>
        </div>
        <table class="TableModify">
            <!-- 基本資料 Start -->
            <tr class="ModifyHead">
                <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTdHead" style="width: 120px">追蹤編號
                    </td>
                    <td class="TableModifyTd styleBlue B">
                        <asp:Literal ID="lt_TraceID" runat="server">系統自動編號</asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 需求者
                    </td>
                    <td class="TableModifyTd">
                        <asp:DropDownListGP ID="ddl_Dept" runat="server">
                        </asp:DropDownListGP>
                        <select id="ddl_Employees"></select>
                        <asp:TextBox ID="tb_EmpValue" runat="server" Style="display: none;" ToolTip="動態產生選單會造成EventValidation 安全性問題，所以將值帶進此欄位"></asp:TextBox>
                        <asp:TextBox ID="tb_Email" runat="server" Style="display: none;" ToolTip="需求者的EMail,發結案通知信時使用"></asp:TextBox>

                        &nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_Dept" runat="server" ControlToValidate="ddl_Dept"
                            Display="Dynamic" ErrorMessage="-&gt; 請選擇「部門」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfv_ddl_Employees" runat="server" ControlToValidate="tb_EmpValue"
                            Display="Dynamic" ErrorMessage="-&gt; 請選擇「人員」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 問題類別
                    </td>
                    <td class="TableModifyTd">
                        <asp:DropDownList ID="ddl_Req_Class" runat="server"></asp:DropDownList>
                        &nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_Req_Class" runat="server" ControlToValidate="ddl_Req_Class"
                            Display="Dynamic" ErrorMessage="-&gt; 請選擇「問題類別」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 填寫主旨
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_Help_Subject" runat="server" MaxLength="40" Width="400px"></asp:TextBox>
                        <span class="SiftLight">(字數上限: 40 字)</span>
                        &nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Help_Subject" runat="server" ControlToValidate="tb_Help_Subject"
                            Display="Dynamic" ErrorMessage="-&gt; 請填寫「主旨」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">有圖有真相
                    </td>
                    <td class="TableModifyTd">
                        <asp:FileUpload ID="fu_Pic" runat="server" />
                        <span class="SiftLight">(可一次上傳多筆, 檔案限制.jpg .png .docx .xlsx .pptx .pdf)</span>
                        <div>
                            <asp:ListView ID="lvDataList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="4" OnItemCommand="lvDataList_ItemCommand">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td align="center" valign="top" width="25%">
                                        <div>
                                            <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                                                OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                                            <asp:HiddenField ID="hf_PicID" runat="server" Value='<%#Eval("AttachID") %>' />
                                            <asp:HiddenField ID="hf_OldFile" runat="server" Value='<%#Eval("AttachFile") %>' />
                                        </div>
                                        <div>
                                            <%#PicUrl(Eval("AttachFile").ToString(), Eval("AttachFile_Org").ToString(), true)%>
                                        </div>
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td></td>
                                </EmptyItemTemplate>
                            </asp:ListView>
                        </div>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 詳細說明
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_Help_Content" runat="server" Width="90%" Rows="10" TextMode="MultiLine"
                            MaxLength="500" placeholder="請填寫「詳細說明」，愈詳細愈好！"></asp:TextBox>
                        <br />
                        <span class="SiftLight">(字數上限: 500 字)</span>
                        &nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Help_Content" runat="server" ControlToValidate="tb_Help_Content"
                            Display="Dynamic" ErrorMessage="-&gt; 請填寫「詳細說明」，愈詳細愈好！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">處理狀態
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Help_Status" runat="server" Text="資料新增中" CssClass="styleGraylight"></asp:Label>
                    </td>
                </tr>
                <!-- 主管同意欄 -->
                <asp:PlaceHolder ID="ph_Agree" runat="server" Visible="false">
                    <tr>
                        <td class="TableModifyTdHead">主管同意
                        </td>
                        <td class="TableModifyTd">
                            <asp:RadioButtonList ID="rbl_IsAgree" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="N" Selected="True">否</asp:ListItem>
                                <asp:ListItem Value="Y">是</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">同意者/時間
                        </td>
                        <td class="TableModifyTd styleGreen">
                            <asp:Literal ID="lt_Agree_Who" runat="server"></asp:Literal>&nbsp;&nbsp;|&nbsp;&nbsp;
                            <asp:Literal ID="lt_Agree_Time" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </tbody>
            <!-- 基本資料 End -->

            <!-- 轉寄通知 Start -->
            <tr class="ModifyHead">
                <td colspan="4">轉寄通知<em class="TableModifyTitleIcon"></em>
                    &nbsp;(<span class="styleRed">**** 此功能會 "寄EMail" 給勾選的人員，請謹慎使用 ****</span>)
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">通知名單</td>
                <td class="TableModifyTd">
                    <asp:PlaceHolder ID="ph_MailItem" runat="server">
                        <div>
                            <input type="button" id="showAll" class="btn btn-default" value="展開" />
                            <input type="button" id="hideAll" class="btn btn-default" value="折疊" />
                        </div>
                        <ul id="myTree" class="ztree">
                        </ul>
                        <asp:TextBox ID="tb_Emps" runat="server" Style="display: none"></asp:TextBox>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_MailList" runat="server">
                        <asp:ListView ID="lv_MailList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table class="List1" width="100%">
                                    <thead>
                                        <tr class="tdHead">
                                            <th style="width: 60%">EMail</th>
                                            <th>名稱</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%#Eval("Mail") %>
                                    </td>
                                    <td>
                                        <%#Eval("Label") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </asp:PlaceHolder>
                </td>
            </tr>
            <!-- 轉寄通知 End -->

            <!-- 回覆資料 Start -->
            <asp:PlaceHolder ID="ph_Reply" runat="server" Visible="false">
                <tbody>
                    <tr class="ModifyHead">
                        <td colspan="4">回覆資料<em class="TableModifyTitleIcon"></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">處理工時
                        </td>
                        <td class="TableModifyTd">
                            <asp:TextBox ID="tb_Reply_Hours"
                                runat="server" MaxLength="4" Style="text-align: center;" type="number" step="0.5" min="0.5"></asp:TextBox>
                            hr(s)
                            <asp:CompareValidator ID="cv_tb_Reply_Hours" runat="server" ControlToValidate="tb_Reply_Hours"
                                Display="Dynamic" ErrorMessage="-&gt; 請輸入數字！" Operator="DataTypeCheck" Type="Double"
                                ForeColor="Red"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">處理回覆
                        </td>
                        <td class="TableModifyTd">
                            <asp:TextBox ID="tb_Reply_Content" runat="server" Width="90%" Rows="7" TextMode="MultiLine"
                                MaxLength="500"></asp:TextBox>
                            <br />
                            <span class="SiftLight">(字數上限: 500 字)</span>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">回覆日期
                        </td>
                        <td class="TableModifyTd">
                            <asp:TextBox ID="tb_Reply_Date" runat="server" Style="text-align: center" Width="80px"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </asp:PlaceHolder>
            <!-- 回覆資料 End -->

            <!-- 維護資訊 Start -->
            <tr class="ModifyHead">
                <td colspan="4">維護資訊<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 100px">維護資訊
                </td>
                <td class="TableModifyTd" colspan="3">
                    <table cellpadding="3" border="0">
                        <tr>
                            <td align="right" width="100px">建立者：
                            </td>
                            <td class="styleGreen" width="200px">
                                <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                            <td align="right" width="100px">建立時間：
                            </td>
                            <td class="styleGreen" width="250px">
                                <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">最後修改者：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Who" runat="server"></asp:Literal>
                            </td>
                            <td align="right">最後修改時間：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Time" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!-- 維護資訊 End -->
        </table>
        <div class="SubmitAreaS">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="text-align: left">
                        <a href="<%=Page_SearchUrl %>" class="btnBlock colorGray">返回列表</a>
                    </td>
                    <td style="text-align: right">
                        <input type="button" id="doSave" value="存檔" class="btnBlock colorBlue" style="width: 72px;" />
                        <asp:Button ID="btn_Save" runat="server" Text="存檔" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />

                        <asp:Button ID="btn_onTop" runat="server" Text="設為置頂" OnClick="btn_onTop_Click"
                            CssClass="btnBlock colorGreen" CausesValidation="false" Visible="false" />

                        <asp:Button ID="btn_Inform" runat="server" Text="回覆並通知" OnClick="btn_Inform_Click"
                            CssClass="btnBlock colorCoffee" OnClientClick="return confirm('回覆並通知給需求者?')" Visible="false" />

                        <asp:Button ID="btn_Done" runat="server" Text="結案" Width="72px" OnClick="btn_Done_Click"
                            CssClass="btnBlock colorRed" OnClientClick="return confirm('是否確定結案!?')" Visible="false" />
                        <asp:Button ID="btn_Back" runat="server" Text="已後悔" Width="72px" OnClick="btn_Back_Click"
                            CssClass="btnBlock colorCoffee" OnClientClick="return confirm('狀態將設為「處理中」')" Visible="false" />
                        <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                            ShowMessageBox="true" ValidationGroup="Add" />
                    </td>
                </tr>
            </table>

        </div>
        <div class="ListIllusArea">
            <div class="JQ-ui-state-default">
                <span id="notice" class="JQ-ui-icon ui-icon-info"></span>&nbsp;<span class="B Font13">功能說明：</span><br />
                <table class="List1" width="100%">
                    <tr class="TrGray">
                        <td>
                            <ul style="list-style-type: decimal;">
                                <li>處理狀態為「待處理」時，資料可任意修改。</li>
                                <li>處理狀態為「處理中/已結案」時，資料凍結無法修改。</li>
                                <li>超過1024px的圖片將會自動等比例壓縮。</li>
                            </ul>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
