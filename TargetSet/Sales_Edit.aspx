<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sales_Edit.aspx.cs" Inherits="Sales_Edit" %>

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
                <%=Application["WebName"]%></a>&gt;<a>目標設定</a>&gt;<span>業務目標設定</span>
        </div>
        <div class="h2Head">
            <h2>業務目標設定 - <strong class="styleRed"><%=fn_Desc.TargetTypeDesc(Param_Type) %></strong></h2>
        </div>
        <asp:Panel ID="pl_Message" runat="server" CssClass="ListIllusArea BgGray">
            <div class="JQ-ui-state-error">
                <div class="styleEarth">
                    <span class="JQ-ui-icon ui-icon-info"></span>
                    <asp:Literal ID="lt_MessageTxt" runat="server"></asp:Literal>
                </div>
            </div>
        </asp:Panel>
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">業務目標<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <!-- 資料設定 Start -->
            <tbody>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 100px">
                        <em>(*)</em> 部門
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <%--  <asp:DropDownList ID="ddl_ShipFrom" runat="server">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfv_ddl_ShipFrom" runat="server" ErrorMessage="-&gt; 請填入「出貨地」!"
                        ControlToValidate="ddl_ShipFrom" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>--%>
                        <asp:DropDownListGP ID="ddl_Dept" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Dept_SelectedIndexChanged">
                        </asp:DropDownListGP>
                        <asp:RequiredFieldValidator ID="rfv_ddl_Dept" runat="server" ErrorMessage="-&gt; 請填入「部門」!"
                            ControlToValidate="ddl_Dept" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px"></td>
                    <td class="TableModifyTd"></td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 年 / 月
                    </td>
                    <td class="TableModifyTd">
                        <asp:DropDownList ID="ddl_Year" runat="server">
                        </asp:DropDownList>
                        年
                    <asp:DropDownList ID="ddl_Mon" runat="server">
                    </asp:DropDownList>
                        月
                    <br />
                        <asp:RequiredFieldValidator ID="rfv_ddl_Year" runat="server" ErrorMessage="-&gt; 請填入「年份」!"
                            ControlToValidate="ddl_Year" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfv_ddl_Mon" runat="server" ErrorMessage="-&gt; 請填入「月份」!"
                            ControlToValidate="ddl_Mon" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 人員
                    </td>
                    <td class="TableModifyTd">
                        <asp:DropDownListGP ID="ddl_Employee" runat="server">
                        </asp:DropDownListGP>
                        <asp:RequiredFieldValidator ID="rfv_ddl_Employee" runat="server" ErrorMessage="-&gt; 請填入「人員」!"
                            ControlToValidate="ddl_Employee" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        &nbsp;<span class="styleBluelight">目前人員：<asp:Label ID="lb_nowStaff" runat="server"
                            CssClass="B"><%=this.ddl_Employee.SelectedItem.Text %></asp:Label></span> &nbsp;
                    <asp:Button ID="btn_Step1" runat="server" Text="設定新目標" ValidationGroup="Add" OnClick="btn_Step1_Click"
                        CssClass="btnBlock colorRed" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Setting" runat="server" Visible="false">
                    <tr>
                        <td class="TableModifyTdHead">設定期限
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Label ID="lb_StartTime" runat="server" CssClass="styleGreen"></asp:Label>
                            ~
                        <asp:Label ID="lb_EndTime" runat="server" CssClass="styleGreen"></asp:Label>
                            &nbsp;(目前狀態：<asp:Label ID="lb_SetStatus" runat="server" CssClass="styleRed B Font13"
                                Text="設定中.."></asp:Label>)
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead styleBlue">
                            <%=this.ddl_Year.SelectedValue %>-<%=this.ddl_Mon.SelectedValue %>
                            <br />
                            目標金額
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <table class="List1" width="100%">
                                <tr class="tdHead">
                                    <td style="width: 80px;">&nbsp;
                                    </td>
                                    <td>部門目標<br />
                                        (銷售金額)
                                    </td>
                                    <td>部門目標<br />
                                        (接單金額)
                                    </td>
                                    <td>部門挑戰金額
                                    </td>
                                    <td>個人目標<br />
                                        <span class="styleBluelight B">(銷售金額)</span>
                                    </td>
                                    <td>個人目標<br />
                                        (接單金額)
                                    </td>
                                </tr>
                                <tr align="center">
                                    <td>
                                        <span class="styleGraylight">(USD)</span>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_Amount_USD" runat="server"><%=Amount_USD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_OrdAmount_USD" runat="server"><%=OrdAmount_USD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_ChAmount_USD" runat="server"><%=ChAmount_USD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_Amount_USD" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_Amount_USD" runat="server" ControlToValidate="tb_Amount_USD"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_OrdAmount_USD" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_OrdAmount_USD" runat="server" ControlToValidate="tb_OrdAmount_USD"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr align="center">
                                    <td>
                                        <span class="styleGraylight">(NTD)</span>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_Amount_NTD" runat="server"><%=Amount_NTD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_OrdAmount_NTD" runat="server"><%=OrdAmount_NTD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_ChAmount_NTD" runat="server"><%=ChAmount_NTD%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_Amount_NTD" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_Amount_NTD" runat="server" ControlToValidate="tb_Amount_NTD"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_OrdAmount_NTD" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_OrdAmount_NTD" runat="server" ControlToValidate="tb_OrdAmount_NTD"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr align="center">
                                    <td>
                                        <span class="styleGraylight">(RMB)</span>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_Amount_RMB" runat="server"><%=Amount_RMB%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_OrdAmount_RMB" runat="server"><%=OrdAmount_RMB%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lb_ChAmount_RMB" runat="server"><%=ChAmount_RMB%></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_Amount_RMB" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_Amount_RMB" runat="server" ControlToValidate="tb_Amount_RMB"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_OrdAmount_RMB" runat="server" Width="100px" MaxLength="13" ValidationGroup="Add"></asp:TextBox>
                                        <asp:CompareValidator ID="cv_tb_OrdAmount_RMB" runat="server" ControlToValidate="tb_OrdAmount_RMB"
                                            Display="Dynamic" ErrorMessage="-&gt; 請輸入整數數字！" Operator="DataTypeCheck" Type="Integer"
                                            ForeColor="Red" ValidationGroup="Add"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </tbody>
            <!-- 資料設定 End -->
        </table>
        <div class="SubmitAreaS">
            <asp:Button ID="btn_Save" runat="server" Text="新增" Width="90px" CssClass="btnBlock colorBlue"
                ValidationGroup="Add" OnClick="btn_Save_Click" Visible="false" />
            <a href="<%=PageUrl_byYear %>" class="btnBlock colorGray">重置</a>
            <a href="<%=Session["BackListUrl"] %>" class="btnBlock colorGray">返回列表</a>

            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
            <asp:HiddenField ID="hf_UID" runat="server" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                ShowMessageBox="true" ValidationGroup="Add" />
        </div>
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="4">已設定目標<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <!-- 今年 已設定 Start -->
            <tr>
                <td class="TableModifyTdHead B" style="width: 80px">
                    <asp:Label ID="lb_SetYear" runat="server" CssClass="styleRed"><%=this.ddl_Year.SelectedValue %></asp:Label>
                </td>
                <td class="TableModifyTd" colspan="3">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
                        OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="List1" width="100%">
                                <tr class="tdHead">
                                    <td width="60px" rowspan="2">月份
                                    </td>
                                    <td colspan="3">銷售
                                    </td>
                                    <td colspan="3">接單
                                    </td>
                                    <td width="80px" rowspan="2">功能選項
                                    </td>
                                </tr>
                                <tr class="tdHead">
                                    <td>USD
                                    </td>
                                    <td>NTD
                                    </td>
                                    <td>RMB
                                    </td>
                                    <td>USD
                                    </td>
                                    <td>NTD
                                    </td>
                                    <td>RMB
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
                                    <asp:PlaceHolder ID="ph_Edit" runat="server" Visible="true"><a class="Edit" href="Sales_Edit.aspx?t=<%=Param_Type %>&EditID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("UID").ToString()))%>">修改</a><br />
                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                                            OnClientClick="return confirm('是否確定刪除本月設定!?')">刪除</asp:LinkButton>
                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%# Eval("UID")%>' />
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="ph_block" runat="server" Visible="false"><span class="styleRed">已截止設定</span></asp:PlaceHolder>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <span class="styleGraylight">-- 未設定目標 --</span>
                        </EmptyDataTemplate>
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
                                    <td width="60px" rowspan="2">月份</td>
                                    <td colspan="3">銷售</td>
                                    <td colspan="3">接單</td>
                                </tr>
                                <tr class="tdHead">
                                    <td>USD</td>
                                    <td>NTD</td>
                                    <td>RMB</td>
                                    <td>USD</td>
                                    <td>NTD</td>
                                    <td>RMB</td>
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
                            <span class="styleGraylight">-- 未設定目標 --</span>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </td>
            </tr>
            <!-- 去年 已設定 End -->
        </table>
    </form>
</body>
</html>
