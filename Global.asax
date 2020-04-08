<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 應用程式啟動時執行的程式碼
        Application["WebName"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebName"];
        Application["DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"];
        Application["WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        Application["File_DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"];
        Application["File_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"];
        Application["RefUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"];
        Application["CDN_Url"] = System.Web.Configuration.WebConfigurationManager.AppSettings["CDN_Url"];
    }

    void Application_End(object sender, EventArgs e)
    {
        //  應用程式關閉時執行的程式碼

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 發生未處理錯誤時執行的程式碼

    }

    void Session_Start(object sender, EventArgs e)
    {
        // 啟動新工作階段時執行的程式碼

    }

    void Session_End(object sender, EventArgs e)
    {
        // 工作階段結束時執行的程式碼。 
        // 注意: 只有在 Web.config 檔將 sessionstate 模式設定為 InProc 時，
        // 才會引發 Session_End 事件。如果將工作階段模式設定為 StateServer 
        // 或 SQLServer，就不會引發這個事件。

    }

</script>
