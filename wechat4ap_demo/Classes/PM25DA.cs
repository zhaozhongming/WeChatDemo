using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public class PM25DA
    {
        private static string ConnectionString = "Server=tcp:gmjcs98a6g.database.windows.net,1433;Database=Wechat4AP;User ID=zhongming@gmjcs98a6g;Password=Yunqi1126;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        public static string GetPM25()
        {
            string retString = string.Empty;
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var pm25 = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {

                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = "select top 1 pm25 from pm25 order by modifiedtime desc";

                        pm25 = Convert.ToInt16(command.ExecuteScalar());


                    }
                }
            }
            catch{ }
            return "当前PM2.5指数：" + pm25.ToString();
        }
    }
}