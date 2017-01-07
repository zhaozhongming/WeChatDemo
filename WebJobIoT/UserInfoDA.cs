using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public class UserInfoDA
    {
        string ConnectionString = "Server=tcp:gmjcs98a6g.database.windows.net,1433;Database=Wechat4AP;User ID=zhongming@gmjcs98a6g;Password=Yunqi1126;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        public int UpdateUserLocation(string userId, string userName, decimal location_X, decimal location_Y)
        {
            int retCode = -1;

            string user = GetUserLocation(userId);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "wechat4ap_userinfo_upd";

                    command.Parameters.Add("@UserId", SqlDbType.NVarChar, 150).Value = userId;
                    
                    if (user.Equals("[]"))//new user
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 150).Value = userName; 
                    else
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 150).Value = DBNull.Value;//existing user, do not update username
                    command.Parameters.Add("@Location_X", SqlDbType.Decimal).Value = location_X;
                    command.Parameters.Add("@Location_Y", SqlDbType.Decimal).Value = location_Y;
                    retCode = command.ExecuteNonQuery();
                }

            } 
            return retCode;
        }

        public string GetUserLocation(string userId)
        {
            //IDataReader dataReader = null;
            //string retString = string.Empty;
            //JavaScriptSerializer Jss = new JavaScriptSerializer();
            //List<UserInfo> userInfoList = new List<UserInfo>();

            //using (SqlConnection connection = new SqlConnection(ConnectionString))
            //{

            //    connection.Open();

            //    using (SqlCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandType = System.Data.CommandType.StoredProcedure;
            //        command.CommandText = "wechat4ap_userinfo_sel";
            //        object param = userId;
            //        if (string.IsNullOrEmpty(userId)) param = DBNull.Value;
            //        command.Parameters.Add("@UserId", SqlDbType.NVarChar, 150).Value = param;

            //        dataReader = command.ExecuteReader();

            //        while (dataReader.Read())
            //        {
            //            UserInfo user = new UserInfo();
            //            user.UserId = dataReader.GetString(0);
            //            user.UserName = dataReader.GetString(1);
            //            user.Location_X = dataReader.GetDecimal(2).ToString();
            //            user.Location_Y = dataReader.GetDecimal(3).ToString();
            //            userInfoList.Add(user);
            //        }
            //    }
            //}
            
            //StringBuilder sb = new StringBuilder();

            //Jss.Serialize(userInfoList, sb);
            return "";
        }
    }
}