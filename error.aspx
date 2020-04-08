<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Threading" %>
<script runat="server">
    void Page_Load()
    {
        byte[] delay = new byte[1];
        RandomNumberGenerator prng = new RNGCryptoServiceProvider();

        prng.GetBytes(delay);
        Thread.Sleep((int)delay[0]);

        IDisposable disposable = prng as IDisposable;
        if (disposable != null) { disposable.Dispose(); }
    }
</script>
<html>
<head id="Head1" runat="server">
    <title>壞掉啦</title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="MainArea">
    <div style="background: url('<%=Application["WebUrl"]%>images/error.jpg') no-repeat; width: 872px; height: 570px">
        <div class="styleBlack Font24 B" style="padding: 30px 0px 0px 220px">
            恭喜，您把網頁弄壞了。
        </div>
        <div class="styleGreen Font13 B" style="text-align: right; padding: 20px 220px 0px 0px">
            此頁面不存在或者發生了其他可怕的錯誤。
        </div>
    </div>
</body>
</html>
