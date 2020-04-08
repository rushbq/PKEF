<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ToyMsg_Edit.aspx.cs" Inherits="ToyMsg_Edit" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
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
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <%-- 連動式選單(轉寄對象) Start --%>
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
                var GetText = $('#ddl_Employees option:selected').text();
                var GetVal = $('#ddl_Employees option:selected').val();

                //填入選擇的人員
                $("#tb_myEmpItemName").val(GetText);   //中文名稱
                $("#tb_myEmpItemEmail").val(GetVal);  //UserID

                //新增動態項目
                Add_EmpItem();

            });

            //若部門有帶預設值, 則自動觸發事件
            $('select#ddl_Dept').trigger("change");
        });

        /* 取得部門人員 - 連動選單 Start */
        function GetEmployees(DeptId) {
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
                        myMenu.append($('<option></option>').val(item.Email).text('(' + item.UserId + ') ' + item.UserName))
                    });

                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    //alert("無法取得人員選單\n\r" + err);
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
    <script>
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_EmpItem() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjValName = $("#tb_myEmpItemName").val();
            var ObjVal = $("#tb_myEmpItemEmail").val();
            if (ObjVal == "") {
                alert('無法取得人員');
                return;
            }

            var NewItem = '<li id="li_Emp' + ObjId + '" style="padding-top:5px;">';
            NewItem += '<input type="hidden" class="Empitem_Name" value="' + ObjValName + '" />';
            NewItem += '<input type="hidden" class="Empitem_Val" value="' + ObjVal + '" />';
            NewItem += '<a href="javascript:Delete_Item(\'Emp' + ObjId + '\');" class="btn btn-success">' + ObjValName + '&nbsp;<span class="glyphicon glyphicon-trash"></span></a>';
            NewItem += '</li>';

            //將項目append到指定控制項
            $("#myEmpItemView").append(NewItem);
        }

        /* 取得各項目欄位值*/
        function Get_EmpItem() {
            //取得控制項, ServerSide
            var fld_itemName = $("#val_EmpName");
            var fld_itemVal = $("#val_EmpEmail");

            //清空欄位值
            fld_itemName.val('');
            fld_itemVal.val('');

            //巡覽項目, 填入值
            $("#myEmpItemView li .Empitem_Name").each(
               function (i, elm) {
                   var OldCont = fld_itemName.val();
                   if (OldCont == '') {
                       fld_itemName.val($(elm).val());
                   } else {
                       fld_itemName.val(OldCont + ',' + $(elm).val());
                   }
               }
           );
            $("#myEmpItemView li .Empitem_Val").each(
                function (i, elm) {
                    var OldCont = fld_itemVal.val();
                    if (OldCont == '') {
                        fld_itemVal.val($(elm).val());
                    } else {
                        fld_itemVal.val(OldCont + ',' + $(elm).val());
                    }
                }
            );

        }


        /* 共用 - 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }

        /* 共用 - 時間function */
        Date.prototype.Format = function (fmt) { //author: meizz
            var o = {
                "M+": this.getMonth() + 1,                 //月份
                "d+": this.getDate(),                    //日
                "h+": this.getHours(),                   //小時
                "m+": this.getMinutes(),                 //分
                "s+": this.getSeconds(),                 //秒
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度
                "S": this.getMilliseconds()             //毫秒
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }
    </script>
    <script>
        /* 自訂Email */
        $(function () {
            //新增鈕
            $("#newEmail").click(function () {
                var myEmailName = $("#tb_myEmailName").val();
                var myEmail = $("#tb_myEmail").val();

                if (IsEmail(myEmail) == false) {
                    alert('輸入格式錯誤');
                    return false;
                }

                if (myEmail != '') {
                    $("#tb_myOtherItemName").val(myEmailName);
                    $("#tb_myOtherItemVal").val(myEmail);
                    Add_OtherItem();
                }

            });

            //Enter偵測
            $("#tb_myEmail").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#newEmail").trigger("click");
                    e.preventDefault();
                }
            });


        });

        function IsEmail(email) {
            var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            return regex.test(email);
        }

        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_OtherItem() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjValName = $("#tb_myOtherItemName").val();
            var ObjVal = $("#tb_myOtherItemVal").val();
            if (ObjValName == "" || ObjVal == "") {
                alert('未填入名稱或Email!');
                return;
            }

            var NewItem = '<li id="li_Other' + ObjId + '" style="padding-top:5px;">';
            NewItem += '<input type="hidden" class="Otheritem_Name" value="' + ObjValName + '" />';
            NewItem += '<input type="hidden" class="Otheritem_Val" value="' + ObjVal + '" />';
            NewItem += '<a href="javascript:Delete_Item(\'Other' + ObjId + '\');" class="btn btn-success">' + ObjValName + '&nbsp;<span class="glyphicon glyphicon-trash"></span></a>';
            NewItem += '</li>';

            //將項目append到指定控制項
            $("#myOtherItemView").append(NewItem);
        }

        /* 取得各項目欄位值*/
        function Get_OtherItem() {
            //取得控制項, ServerSide
            var fld_itemName = $("#val_OtherName");
            var fld_itemVal = $("#val_OtherEmail");

            //清空欄位值
            fld_itemName.val('');
            fld_itemVal.val('');

            //巡覽項目, 填入值
            $("#myOtherItemView li .Otheritem_Name").each(
                function (i, elm) {
                    var OldCont = fld_itemName.val();
                    if (OldCont == '') {
                        fld_itemName.val($(elm).val());
                    } else {
                        fld_itemName.val(OldCont + ',' + $(elm).val());
                    }
                }
            );
            $("#myOtherItemView li .Otheritem_Val").each(
                function (i, elm) {
                    var OldCont = fld_itemVal.val();
                    if (OldCont == '') {
                        fld_itemVal.val($(elm).val());
                    } else {
                        fld_itemVal.val(OldCont + ',' + $(elm).val());
                    }
                }
            );

        }

    </script>

    <!-- 承辦人員 -->
    <%-- 連動式選單(承辦人員) Start --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //部門選單變動時觸發事件
            $('select#ddl_Dept_Sales').change(function () {
                var GetVal = $('#ddl_Dept_Sales option:selected').val();
                //取得部門人員
                GetSales(GetVal);

            });

            //人員選單變動時觸發事件
            $('select#ddl_Sales').change(function () {
                var GetText = $('#ddl_Sales option:selected').text();
                var GetVal = $('#ddl_Sales option:selected').val();

                //填入選擇的人員
                $("#tb_mySalesItemName").val(GetText);   //中文名稱
                $("#tb_mySalesItemEmail").val(GetVal);  //UserID

                //新增動態項目
                Add_SalesItem();

            });

            //若部門有帶預設值, 則自動觸發事件
            $('select#ddl_Dept_Sales').trigger("change");
        });

        /* 取得部門人員 - 連動選單 Start */
        function GetSales(DeptId) {
            //宣告 - 取得物件,人員
            var myMenu = $('select#ddl_Sales');
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
                        myMenu.append($('<option></option>').val(item.Email).text('(' + item.UserId + ') ' + item.UserName))
                    });

                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    //alert("無法取得人員選單\n\r" + err);
                });
        }


        /* 取得部門人員 - 連動選單 End */
    </script>
    <%-- 連動式選單 End --%>
    <script>
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_SalesItem() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjValName = $("#tb_mySalesItemName").val();
            var ObjVal = $("#tb_mySalesItemEmail").val();
            if (ObjVal == "") {
                alert('無法取得人員');
                return;
            }

            var NewItem = '<li id="li_Sales' + ObjId + '" style="padding-top:5px;">';
            NewItem += '<input type="hidden" class="Salesitem_Name" value="' + ObjValName + '" />';
            NewItem += '<input type="hidden" class="Salesitem_Val" value="' + ObjVal + '" />';
            NewItem += '<a href="javascript:Delete_Item(\'Sales' + ObjId + '\');" class="btn btn-success">' + ObjValName + '&nbsp;<span class="glyphicon glyphicon-trash"></span></a>';
            NewItem += '</li>';

            //將項目append到指定控制項
            $("#mySalesItemView").append(NewItem);
        }

        /* 取得各項目欄位值*/
        function Get_SalesItem() {
            //取得控制項, ServerSide
            var fld_itemName = $("#val_SalesName");
            var fld_itemVal = $("#val_SalesEmail");

            //清空欄位值
            fld_itemName.val('');
            fld_itemVal.val('');

            //巡覽項目, 填入值
            $("#mySalesItemView li .Salesitem_Name").each(
               function (i, elm) {
                   var OldCont = fld_itemName.val();
                   if (OldCont == '') {
                       fld_itemName.val($(elm).val());
                   } else {
                       fld_itemName.val(OldCont + ',' + $(elm).val());
                   }
               }
           );
            $("#mySalesItemView li .Salesitem_Val").each(
                function (i, elm) {
                    var OldCont = fld_itemVal.val();
                    if (OldCont == '') {
                        fld_itemVal.val($(elm).val());
                    } else {
                        fld_itemVal.val(OldCont + ',' + $(elm).val());
                    }
                }
            );
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>業務行銷</a>&gt;<span>科學玩具網站訊息</span>
        </div>
        <div class="h2Head">
            <h2>科學玩具網站訊息</h2>
        </div>
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <!-- 主檔 Start -->
                <tr class="ModifyHead">
                    <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 150px">追蹤編號
                        </td>
                        <td class="TableModifyTd styleBlue B" style="width: 400px">
                            <asp:Literal ID="lt_TraceID" runat="server">系統自動編號</asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">訊息類別 / 處理狀態
                        </td>
                        <td class="TableModifyTd">
                            <asp:Label ID="lb_Class" runat="server" CssClass="label label-danger"></asp:Label>
                            <asp:Label ID="lb_Status" runat="server"></asp:Label>
                            <asp:HiddenField ID="hf_Current_Status" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">留言訊息
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_Message" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">回覆信收件者 <strong>(A)</strong>
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_MemberMail" runat="server"></asp:Literal>
                            &nbsp;
                            <a href="javascript:void(0)" data-toggle="modal" data-target="#myModal">顯示填寫人資料&nbsp;<span class="glyphicon glyphicon-new-window"></span></a>

                            <!-- 會員資料 Modal Start -->
                            <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <h4 class="modal-title" id="myModalLabel">填寫人資料</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row form-horizontal">
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">Email</label>
                                                    <div class="col-xs-10 form-control-static">
                                                        <asp:Literal ID="modal_Email" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">姓</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_FirstName" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">名</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_LastName" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-xs-2 control-label label label-default">電話</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Tel" runat="server"></asp:Literal>
                                                    </div>
                                                    <label class="col-xs-2 control-label label label-default">國家</label>
                                                    <div class="col-xs-4 form-control-static">
                                                        <asp:Literal ID="modal_Country" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- 會員資料 Modal End -->
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">回覆信寄件者
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_ReplyMailSender" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <!-- 轉寄欄位 Start -->
                    <tr class="Must">
                        <td class="TableModifyTdHead" rowspan="3">回覆信轉寄對象 <strong>(B)</strong><br />
                            <em class="styleRed">(不會收到客戶資料)</em></td>
                        <td class="TableModifyTd" colspan="3">
                            <label><u>內部員工</u>&nbsp;<span class="SiftLight">(人員重複,送出表單後會自動排除)</span></label>
                            <div class="form-inline">
                                <asp:DropDownListGP ID="ddl_Dept" runat="server" CssClass="form-control">
                                </asp:DropDownListGP>
                                &nbsp;                                          
                                <select id="ddl_Employees" class="form-control"></select>
                            </div>
                            <div class="hidden-field">
                                <input type="hidden" id="tb_myEmpItemName" />
                                <input type="hidden" id="tb_myEmpItemEmail" />
                                <asp:TextBox ID="val_EmpName" runat="server" Style="display: none;" ToolTip="欄位值集合(Name)"></asp:TextBox>
                                <asp:TextBox ID="val_EmpEmail" runat="server" Style="display: none;" ToolTip="欄位值集合(EMail)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTd" colspan="3">
                            <label><u>自訂名單</u>&nbsp;<span class="SiftLight">(EMail重複,送出表單後會自動排除)</span></label>
                            <div class="form-inline">
                                名稱<input type="text" maxlength="10" id="tb_myEmailName" class="form-control" placeholder="填入名稱" />
                                Email<input type="email" maxlength="60" id="tb_myEmail" class="form-control" placeholder="填入Email" />
                                <a class="btn btn-default" href="javascript:;" id="newEmail">新增</a>
                            </div>

                            <div class="hidden-field">
                                <input type="hidden" id="tb_myOtherItemName" />
                                <input type="hidden" id="tb_myOtherItemVal" />
                                <asp:TextBox ID="val_OtherName" runat="server" Style="display: none;" ToolTip="欄位值集合(Name)"></asp:TextBox>
                                <asp:TextBox ID="val_OtherEmail" runat="server" Style="display: none;" ToolTip="欄位值集合(EMail)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTd" colspan="3">
                            <div class="showResult">
                                <ul class="list-inline" id="myEmpItemView">
                                    <asp:Literal ID="lt_EmpItems" runat="server"></asp:Literal>
                                </ul>
                                <ul class="list-inline" id="myOtherItemView">
                                    <asp:Literal ID="lt_OtherItems" runat="server"></asp:Literal>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <!-- 轉寄欄位 End -->
                    <tr class="Must">
                        <td class="TableModifyTdHead">
                            <em>(*)</em> 信件主旨
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:TextBox ID="tb_Subject" runat="server" CssClass="form-control" MaxLength="100" Width="600px"></asp:TextBox>
                            <span class="SiftLight">(字數上限: 50 字)</span>
                            &nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Subject" runat="server" ControlToValidate="tb_Subject"
                                Display="Dynamic" ErrorMessage="-&gt; 請填寫「信件主旨」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="Must">
                        <td class="TableModifyTdHead">
                            <em>(*)</em> 回覆內文
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:TextBox ID="tb_Reply_Message" runat="server" CssClass="form-control" Width="600px" Rows="15" TextMode="MultiLine"
                                MaxLength="3500"></asp:TextBox>
                            <span class="SiftLight">(字數上限: 3000 字)</span>
                            &nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Reply_Message" runat="server" ControlToValidate="tb_Reply_Message"
                                Display="Dynamic" ErrorMessage="-&gt; 請填寫「回覆內文」！" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfv_tb_Reply_Message1" runat="server" ControlToValidate="tb_Reply_Message"
                                Display="Dynamic" ErrorMessage="-&gt; 請填寫「回覆內文」！" ForeColor="Red" ValidationGroup="Hide"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <!-- 轉寄承辦人員 Start -->
                    <tr class="Must">
                        <td class="TableModifyTdHead" rowspan="2">轉寄承辦人員 <strong>(C)</strong><br />
                            <em class="styleRed">(將會收到客戶資料)</em></td>
                        <td class="TableModifyTd" colspan="3">
                            <div class="form-inline">
                                <asp:DropDownListGP ID="ddl_Dept_Sales" runat="server" CssClass="form-control">
                                </asp:DropDownListGP>
                                &nbsp;                                          
                                <select id="ddl_Sales" class="form-control"></select>
                            </div>
                            <div class="hidden-field">
                                <input type="hidden" id="tb_mySalesItemName" />
                                <input type="hidden" id="tb_mySalesItemEmail" />
                                <asp:TextBox ID="val_SalesName" runat="server" Style="display: none;" ToolTip="欄位值集合(Name)"></asp:TextBox>
                                <asp:TextBox ID="val_SalesEmail" runat="server" Style="display: none;" ToolTip="欄位值集合(EMail)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTd" colspan="3">
                            <div class="showResult">
                                <ul class="list-inline" id="mySalesItemView">
                                    <asp:Literal ID="lt_SalesItems" runat="server"></asp:Literal>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <!-- 轉寄承辦人員 End -->

                    <tr>
                        <td class="TableModifyTdHead">維護資訊
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <table cellpadding="3" border="0">
                                <tr>
                                    <td align="right" width="100px">建立時間：
                                    </td>
                                    <td class="styleGreen" width="200px">
                                        <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                                    </td>
                                    <td align="right" width="100px">回覆時間：
                                    </td>
                                    <td class="styleGreen" width="250px">
                                        <asp:Literal ID="lt_Reply_Time" runat="server" Text="新增資料中"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                </tbody>
                <!-- 主檔 End -->
            </table>
        </div>
        <div class="SubmitAreaS">
            <input type="button" id="triggerSave" class="btnBlock colorBlue" value="存檔不回覆" />
            <button type="button" id="triggerReply" class="btn btn-danger"><span class="glyphicon glyphicon-send"></span>&nbsp;回覆此信</button>

            <asp:Button ID="btn_Save" runat="server" Text="存檔不回覆" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />

            <asp:Button ID="btn_Reply" runat="server" Text="回覆此信" OnClick="btn_Reply_Click" ValidationGroup="Add" Style="display: none;" />

            <asp:Button ID="btn_Hide" runat="server" Text="作廢信件" OnClick="btn_Hide_Click"
                CssClass="btnBlock colorCoffee" OnClientClick="return confirm('是否確定作廢信件!?')" ValidationGroup="Hide" />

            <button type="button" id="triggerFinish" class="btn btn-success"><span class="glyphicon glyphicon-bookmark"></span>&nbsp;結案</button>
            <asp:Button ID="btn_Finish" runat="server" Text="結案" OnClick="btn_Finish_Click" ValidationGroup="Add" Style="display: none;" />

            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
            <asp:HiddenField ID="hf_ReplyID" runat="server" />
            <asp:HiddenField ID="hf_Status" runat="server" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                ShowMessageBox="true" ValidationGroup="Add" />
        </div>
        <div class="alert alert-info alert-dismissible" role="alert">
            <h4>[按鈕說明]</h4>
            <ul>
                <li>存檔不回覆：儲存已填寫的欄位，不寄發回覆信，狀態設為「處理中」。</li>
                <li>回覆此信：將回覆內容，回覆給 <strong>客戶(A) / 轉寄對象(B) / 承辦人員(C)</strong>，狀態設為「已回覆」。</li>
                <li>此為垃圾信：將此信歸類為垃圾信，狀態設為「垃圾訊息」。</li>
                <li>結案：將回覆內容，回覆給 <strong>轉寄對象(B) / 承辦人員(C)</strong>，狀態設為「結案」。</li>
            </ul>
        </div>

        <script>
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                Get_EmpItem();
                Get_OtherItem();
                Get_SalesItem();
                $('#btn_Save').trigger('click');
            });

            //Click事件, 觸發回覆
            $("#triggerReply").click(function () {
                if (confirm('是否確定回覆?')) {
                    Get_EmpItem();
                    Get_OtherItem();
                    Get_SalesItem();
                    $('#btn_Reply').trigger('click');
                }
            });

            //Click事件, 觸發結案
            $("#triggerFinish").click(function () {
                if (confirm('是否確定結案?')) {
                    Get_EmpItem();
                    Get_OtherItem();
                    Get_SalesItem();
                    $('#btn_Finish').trigger('click');
                }
            });
        </script>
        <!-- 將來的討論串 -->
        <%--<div class="bq-callout orange">
            <div>
                <p>
                    okkki
                </p>
                <footer>2014/01/30 11:25</footer>
            </div>
        </div>--%>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
