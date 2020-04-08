<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Channel_Edit.aspx.cs" Inherits="Channel_Edit" %>

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
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>目標設定</a>&gt;<span>通路目標設定</span>
    </div>
    <div class="h2Head">
        <h2>
            通路目標設定</h2>
    </div>
    <asp:Panel ID="pl_Message" runat="server" CssClass="ListIllusArea BgGray">
        <div class="JQ-ui-state-error">
            <div class="styleEarth">
                <span class="JQ-ui-icon ui-icon-info"></span>
                <asp:Literal ID="lt_MessageTxt" runat="server"></asp:Literal></div>
        </div>
    </asp:Panel>
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                通路目標<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <!-- 資料設定 Start -->
        <tbody>
            <tr class="Must">
                <td class="TableModifyTdHead" style="width: 100px">
                    <em>(*)</em> 出貨地
                </td>
                <td class="TableModifyTd" style="width: 350px">
                    <asp:DropDownList ID="ddl_ShipFrom" runat="server" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfv_ddl_ShipFrom" runat="server" ErrorMessage="-&gt; 請填入「出貨地」!"
                        ControlToValidate="ddl_ShipFrom" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                </td>
                <td class="TableModifyTdHead" style="width: 100px">
                    <em>(*)</em> 通路別
                </td>
                <td class="TableModifyTd">
                    <asp:DropDownList ID="ddl_Channel" runat="server" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfv_ddl_Channel" runat="server" ErrorMessage="-&gt; 請填入「通路別」!"
                        ControlToValidate="ddl_Channel" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    <em>(*)</em> 年 / 月
                </td>
                <td class="TableModifyTd">
                    <asp:DropDownList ID="ddl_Year" runat="server" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                    年
                    <asp:DropDownList ID="ddl_Mon" runat="server">
                    </asp:DropDownList>
                    月
                    <br />
                    <asp:RequiredFieldValidator ID="rfv_ddl_Year" runat="server" ErrorMessage="-&gt; 請填入「年份」!"
                        ControlToValidate="ddl_Year" Display="Dynamic" ForeColor="Red" ValidationGroup="Add">
                    </asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfv_ddl_Mon" runat="server" ErrorMessage="-&gt; 請填入「月份」!"
                        ControlToValidate="ddl_Mon" Display="Dynamic" ForeColor="Red" ValidationGroup="Add">
                    </asp:RequiredFieldValidator>
                </td>
                <td class="TableModifyTdHead styleBlue">
                    目標金額
                </td>
                <td class="TableModifyTd">
                    <table class="TableS3">
                        <tr>
                            <td class="TS3Head TableS3Dark styleBluelight B">
                                銷售
                            </td>
                            <td class="TS3Head TableS3Dark">
                                接單
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="padding-bottom: 4px">
                                    <span class="styleGraylight">(USD)</span>&nbsp;
                                    <asp:TextBox ID="tb_Amount_USD" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_Amount_USD" runat="server" ControlToValidate="tb_Amount_USD"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                                <div style="padding-bottom: 4px">
                                    <span class="styleGraylight">(NTD)</span>&nbsp;
                                    <asp:TextBox ID="tb_Amount_NTD" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_Amount_NTD" runat="server" ControlToValidate="tb_Amount_NTD"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                                <div>
                                    <span class="styleGraylight">(RMB)</span>&nbsp;
                                    <asp:TextBox ID="tb_Amount_RMB" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_Amount_RMB" runat="server" ControlToValidate="tb_Amount_RMB"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                            </td>
                            <td>
                                <div style="padding-bottom: 4px">
                                    <span class="styleGraylight">(USD)</span>&nbsp;
                                    <asp:TextBox ID="tb_OrdAmount_USD" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_OrdAmount_USD" runat="server" ControlToValidate="tb_OrdAmount_USD"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                                <div style="padding-bottom: 4px">
                                    <span class="styleGraylight">(NTD)</span>&nbsp;
                                    <asp:TextBox ID="tb_OrdAmount_NTD" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_OrdAmount_NTD" runat="server" ControlToValidate="tb_OrdAmount_NTD"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                                <div>
                                    <span class="styleGraylight">(RMB)</span>&nbsp;
                                    <asp:TextBox ID="tb_OrdAmount_RMB" runat="server" Width="150px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                    <asp:CompareValidator ID="cv_tb_OrdAmount_RMB" runat="server" ControlToValidate="tb_OrdAmount_RMB"
                                        Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                        ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </tbody>
        <!-- 資料設定 End -->
    </table>
    <div class="SubmitAreaS">
        <asp:Button ID="btn_Save" runat="server" Text="新增" Width="90px" CssClass="btnBlock colorBlue"
            ValidationGroup="Add" OnClick="btn_Save_Click" />
        <input onclick="location.href = '<%=PageUrl_byYear %>';" type="button" style="width: 90px;"
            class="btnBlock colorGray" value="重置" />
        <input onclick="location.href = '<%=Session["BackListUrl"] %>';" type="button" style="width: 90px;"
            class="btnBlock colorGray" value="返回列表" />
        <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
        <asp:HiddenField ID="hf_UID" runat="server" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="Add" />
    </div>
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                已設定目標<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <!-- 今年 已設定 Start -->
        <tr>
            <td class="TableModifyTdHead B" style="width: 80px">
                <asp:Label ID="lb_SetYear" runat="server" CssClass="styleRed"><%=this.ddl_Year.SelectedValue %></asp:Label>
            </td>
            <td class="TableModifyTd" colspan="3">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                    <LayoutTemplate>
                        <table class="List1" width="100%">
                            <tr class="tdHead">
                                <td width="60px" rowspan="2">
                                    月份
                                </td>
                                <td colspan="3">
                                    銷售
                                </td>
                                <td colspan="3">
                                    接單
                                </td>
                                <td width="80px" rowspan="2">
                                    功能選項
                                </td>
                            </tr>
                            <tr class="tdHead">
                                <td>
                                    USD
                                </td>
                                <td>
                                    NTD
                                </td>
                                <td>
                                    RMB
                                </td>
                                <td>
                                    USD
                                </td>
                                <td>
                                    NTD
                                </td>
                                <td>
                                    RMB
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="trItem" runat="server" align="center">
                            <td>
                                <%#Eval("SetMonth")%>
                            </td>
                            <%-- 銷售 --%>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_USD").ToString())%>
                            </td>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_NTD").ToString())%>
                            </td>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_RMB").ToString())%>
                            </td>
                            <%-- 接單 --%>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_USD").ToString())%>
                            </td>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_NTD").ToString())%>
                            </td>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_RMB").ToString())%>
                            </td>
                            <td align="center">
                                <a class="Edit" href="Channel_Edit.aspx?EditID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("UID").ToString()))%>">
                                    修改</a><br />
                                <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                                    OnClientClick="return confirm('是否確定刪除本月設定!?')">刪除</asp:LinkButton>
                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%# Eval("UID")%>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <span class="styleGraylight">-- 未設定目標 --</span></EmptyDataTemplate>
                </asp:ListView>
            </td>
        </tr>
        <!-- 今年 已設定 End -->
        <!-- 去年 已設定 Start -->
        <tr>
            <td class="TableModifyTdHead">
                <%=Convert.ToInt16(this.ddl_Year.SelectedValue) -1%>
            </td>
            <td class="TableModifyTd" colspan="3">
                <asp:ListView ID="lvDataList_LastYear" runat="server" ItemPlaceholderID="ph_Items">
                    <LayoutTemplate>
                        <table class="List1" width="100%">
                            <tr class="tdHead">
                                <td width="60px" rowspan="2">
                                    月份
                                </td>
                                <td colspan="3">
                                    銷售
                                </td>
                                <td colspan="3">
                                    接單
                                </td>
                            </tr>
                            <tr class="tdHead">
                                <td>
                                    USD
                                </td>
                                <td>
                                    NTD
                                </td>
                                <td>
                                    RMB
                                </td>
                                <td>
                                    USD
                                </td>
                                <td>
                                    NTD
                                </td>
                                <td>
                                    RMB
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="trItem" runat="server" align="center">
                            <td>
                                <%#Eval("SetMonth")%>
                            </td>
                            <%-- 銷售 --%>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_USD").ToString())%>
                            </td>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_NTD").ToString())%>
                            </td>
                            <td class="styleBlue">
                                <%#fn_stringFormat.C_format(Eval("Amount_RMB").ToString())%>
                            </td>
                            <%-- 接單 --%>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_USD").ToString())%>
                            </td>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_NTD").ToString())%>
                            </td>
                            <td>
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_RMB").ToString())%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <span class="styleGraylight">-- 未設定目標 --</span></EmptyDataTemplate>
                </asp:ListView>
            </td>
        </tr>
        <!-- 去年 已設定 End -->
    </table>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
