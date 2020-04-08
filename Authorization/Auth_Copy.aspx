<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_Copy.aspx.cs" Inherits="Auth_Copy" %>

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
    <%-- catcomplete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/catcomplete/catcomplete.js" type="text/javascript"></script>
    <%-- catcomplete End --%>
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <script type="text/javascript">
        $(function () {
            //AutoComplete - 群組
            $("#tb_Group_Name_from").autocomplete('../AC_ADGroups.aspx?Srh_Type=System');
            $("#tb_Group_Name_from").result(
                function (event, data, formatted) {
                    $("#lb_Group_Name_from").text(data[0]);
                    $("#tb_Group_ID_from").val(data[1]);
                    $("#Rev_GroupFrom").show();
                }
	        );
            $("#tb_Group_Name_to").autocomplete('../AC_ADGroups.aspx?Srh_Type=User');
            $("#tb_Group_Name_to").result(
                function (event, data, formatted) {
                    $("#lb_Group_Name_to").text(data[0]);
                    $("#tb_Group_ID_to").val(data[1]);
                    $("#Rev_GroupTo").show();
                }
	        );

            //AutoComplete - 使用者
            //            $("#tb_Profile_Name_from").autocomplete('../AC_ADUsers.aspx?Srh_Type=System');
            //            $("#tb_Profile_Name_from").result(
            //                function (event, data, formatted) {
            //                    $("#lb_Profile_Name_from").text(data[0]);
            //                    $("#tb_Profile_ID_from").val(data[1]);
            //                    $("#Rev_ProfileFrom").show();
            //                }
            //	        );
            //            $("#tb_Profile_Name_to").autocomplete('../AC_ADUsers.aspx?Srh_Type=User');
            //            $("#tb_Profile_Name_to").result(
            //                function (event, data, formatted) {

            //                }
            //	        );
            /* Autocomplete - 群組分類(AD使用者) */
            $("#tb_Profile_Name_from").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_ADUsers.aspx",
                        data: {
                            q: request.term,
                            type: "System"
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category,
                                    Guid: item.Guid
                                }
                            }));
                        }
                    });
                },
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    $("#lb_Profile_Name_from").text(ui.item.value);
                    $("#tb_Profile_ID_from").val(ui.item.Guid);
                    $("#Rev_ProfileFrom").show();
                }
            });
            $("#tb_Profile_Name_to").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_ADUsers.aspx",
                        data: {
                            q: request.term,
                            type: "User"
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category,
                                    Guid: item.Guid
                                }
                            }));
                        }
                    });
                },
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    $("#lb_Profile_Name_to").text(ui.item.value);
                    $("#tb_Profile_ID_to").val(ui.item.Guid);
                    $("#Rev_ProfileTo").show();
                }
            });

        });

        //清除Autocomplete結果欄位
        function clear_Field(objName) {
            switch (objName) {
                case 'GroupFrom':
                    $("#lb_Group_Name_from").text('');
                    $("#tb_Group_Name_from").val('');
                    $("#tb_Group_ID_from").val('');
                    $("#Rev_GroupFrom").hide();
                    break;

                case 'GroupTo':
                    $("#lb_Group_Name_to").text('');
                    $("#tb_Group_Name_to").val('');
                    $("#tb_Group_ID_to").val('');
                    $("#Rev_GroupTo").hide();
                    break;

                case 'ProfileFrom':
                    $("#lb_Profile_Name_from").text('');
                    $("#tb_Profile_Name_from").val('');
                    $("#tb_Profile_ID_from").val('');
                    $("#Rev_ProfileFrom").hide();
                    break;

                case 'ProfileTo':
                    $("#lb_Profile_Name_to").text('');
                    $("#tb_Profile_Name_to").val('');
                    $("#tb_Profile_ID_to").val('');
                    $("#Rev_ProfileTo").hide();
                    break;

                default:

            }
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>權限管理</a>&gt;<span>權限複製</span>
    </div>
    <div class="h2Head">
        <h2>
            權限複製</h2>
    </div>
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                [群組]<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" style="width: 100px">
                複製來源
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Group_Name_from" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
                <asp:TextBox ID="tb_Group_ID_from" runat="server" Style="display: none;" MaxLength="40"
                    ValidationGroup="UserGroup"></asp:TextBox>
                <img id="Rev_GroupFrom" src="../images/delete.png" onclick="clear_Field('GroupFrom');"
                    title="清除" alt="清除" style="display: none; cursor: pointer;">
                <asp:Label ID="lb_Group_Name_from" runat="server" CssClass="styleGreen"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                複製目標
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Group_Name_to" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
                <asp:TextBox ID="tb_Group_ID_to" runat="server" Style="display: none;" MaxLength="40"
                    ValidationGroup="UserGroup"></asp:TextBox>
                <img id="Rev_GroupTo" src="../images/delete.png" onclick="clear_Field('GroupTo');"
                    title="清除" alt="清除" style="display: none; cursor: pointer;">
                <asp:Label ID="lb_Group_Name_to" runat="server" CssClass="styleGreen"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                複製結果
            </td>
            <td class="TableModifyTd styleBlue">
                <asp:Literal ID="lt_Result_Group" runat="server" Text="請於輸入欄內，填入關鍵字後，選擇正確的群組。"></asp:Literal>
                <asp:RequiredFieldValidator ID="rfv_tb_Group_ID_from" runat="server" ErrorMessage="*請選擇正確的「來源」!"
                    ControlToValidate="tb_Group_ID_from" Display="Dynamic" ForeColor="Red" ValidationGroup="UserGroup"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfv_tb_Group_ID_to" runat="server" ErrorMessage="*請選擇正確的「目標」!"
                    ControlToValidate="tb_Group_ID_to" Display="Dynamic" ForeColor="Red" ValidationGroup="UserGroup"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="SubmitAreaS" colspan="2">
                <asp:Button ID="btn_CopyGroup" runat="server" Text="開始複製" ValidationGroup="UserGroup"
                    Width="90px" CssClass="btnBlock colorBlue" OnClick="btn_CopyGroup_Click" />
            </td>
        </tr>
        <tr class="ModifyHead">
            <td colspan="4">
                [使用者]<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" style="width: 100px">
                複製來源
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Profile_Name_from" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
                <asp:TextBox ID="tb_Profile_ID_from" runat="server" Style="display: none;" MaxLength="40"></asp:TextBox>
                <img id="Rev_ProfileFrom" src="../images/delete.png" onclick="clear_Field('ProfileFrom');"
                    title="清除" alt="清除" style="display: none; cursor: pointer;">
                <asp:Label ID="lb_Profile_Name_from" runat="server" CssClass="styleGreen"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                複製目標
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Profile_Name_to" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
                <asp:TextBox ID="tb_Profile_ID_to" runat="server" Style="display: none;" MaxLength="40"></asp:TextBox>
                <img id="Rev_ProfileTo" src="../images/delete.png" onclick="clear_Field('ProfileTo');"
                    title="清除" alt="清除" style="display: none; cursor: pointer;">
                <asp:Label ID="lb_Profile_Name_to" runat="server" CssClass="styleGreen"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                複製結果
            </td>
            <td class="TableModifyTd styleEarth">
                <asp:Literal ID="lt_Result_Profile" runat="server">請於輸入欄內，填入關鍵字後，選擇正確的使用者。</asp:Literal>
                <asp:RequiredFieldValidator ID="rfv_tb_Profile_ID_from" runat="server" ErrorMessage="*請選擇正確的「來源」!"
                    ControlToValidate="tb_Profile_ID_from" Display="Dynamic" ForeColor="Red" ValidationGroup="UserProfile"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfv_tb_Profile_ID_to" runat="server" ErrorMessage="*請選擇正確的「目標」!"
                    ControlToValidate="tb_Profile_ID_to" Display="Dynamic" ForeColor="Red" ValidationGroup="UserProfile"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="SubmitAreaS" colspan="2">
                <asp:Button ID="btn_CopyProfile" runat="server" Text="開始複製" ValidationGroup="UserProfile"
                    Width="90px" CssClass="btnBlock colorBlue" OnClick="btn_CopyProfile_Click" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
