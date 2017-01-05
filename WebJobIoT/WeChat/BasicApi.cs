using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebJobIoT.Classes
{
    public class BasicApi
    {
        public static string SessionAccessToken = "TOKEN";//access_token缓存 其他接口的通行证
        public static string SessionJSTicket = "JSTICKET";
        public static string APPID="wx75ee8ceec612733b";
        public static string APPSECRET = "d4624c36b6795d1d99dcf0547af5443d";

        public BasicApi() { }

        #region 获取access_token缓存

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string respText = CommonMethod.WebRequestPostOrGet(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", APPID, APPSECRET), "");
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            string accessToken = respDic["access_token"].ToString();
            return accessToken;
        }
        #endregion
    } 
}