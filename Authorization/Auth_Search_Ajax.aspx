<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_Search_Ajax.aspx.cs"
    Inherits="Auth_Search_Ajax" %>

<div class="JQ-ui-state-default">
    <span class="styleGreen B"><span class="JQ-ui-icon ui-icon-circle-triangle-e"></span>
        <%=Param_Title %></span> &nbsp;<span class="SiftLight">(點選下方項目，可直接前往設定權限)</span>
</div>
<hr class="MenuSecondHr" />
<div>
    <table class="TableS3" width="100%">
        <tr>
            <td class="TS3Head TableS3Dark" style="height: 30px;">
                <span class="styleBlue B">群組名單</span>
            </td>
        </tr>
        <tbody>
            <tr>
                <td class="SiftItem_Table">
                    <%=Param_GroupNameList%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<div style="height: 30px">
</div>
<div>
    <table class="TableS3" width="100%">
        <tr>
            <td class="TS3Head TableS3Dark" colspan="2" style="height: 30px;">
                <span class="styleBlue B">人員名單</span>
            </td>
        </tr>
        <tbody>
            <tr>
                <td class="SiftItem_Table">
                    <%=Param_UserNameList%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
