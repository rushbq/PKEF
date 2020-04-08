using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Text;

public partial class Create_Xml : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        string ErrMsg = "";

        //判斷資料夾是否存在
        string folder = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\";
        if (fn_Extensions.CheckFolder(folder) == false)
        {
            Response.Write("壞掉了，無法產生資料夾");
            return;
        }

        string Param_GUID = "{faf509ab-006d-4875-96a0-44277fc8990d}";

        //[SQL] - 新增權限
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" IF (SELECT COUNT(*) FROM PKSYS.dbo.User_Profile WHERE (Guid = @Param_GUID)) = 0 ");
            SBSql.AppendLine("  BEGIN ");
            SBSql.AppendLine("   INSERT INTO PKSYS.dbo.User_Profile (Guid, Display_Name, Account_Name) ");
            SBSql.AppendLine("   VALUES (@Param_GUID, '10308', '10308') ");
            SBSql.AppendLine("  END ");
            SBSql.AppendLine(" DELETE FROM User_Profile_Rel_Program WHERE (Guid = @Param_GUID) ");
            SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) VALUES (@Param_GUID, 9900) ");
            SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) VALUES (@Param_GUID, 9901) ");
            SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) VALUES (@Param_GUID, 9902) ");
            SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) VALUES (@Param_GUID, 9903) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Param_GUID", Param_GUID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                Response.Write("壞掉了，無法新增權限到DB");
                return;
            }
        }

        //[XML] - 根目錄
        XElement DataNode = new XElement("Users");
        //[XML] - 新增節點
        DataNode.Add(new XElement("User",
               new XAttribute("Guid", Param_GUID),
               new XAttribute("Display", "Y")
               , new XElement("ProgID", "9900")
               , new XElement("ProgID", "9901")
               , new XElement("ProgID", "9902")
               , new XElement("ProgID", "9903")
               ));
        //[XML] -  產生XML檔案
        XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), DataNode);
        xdoc.Save(folder + @"User_Profile_10308.xml");

        Response.Write("OK");
    }

}