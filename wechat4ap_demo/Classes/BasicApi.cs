using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public class BasicApi
    {
        public static string SessionAccessToken = "TOKEN";//access_token缓存 其他接口的通行证
        public static string SessionJSTicket = "JSTICKET";
        public static string APPID="wx75ee8ceec612733b";
        public static string APPSECRET = "d4624c36b6795d1d99dcf0547af5443d";

        public BasicApi() { }

        #region 获取access_token缓存
        public static string GetTokenSession()
        {
            string TokenSession = "";

            if (System.Web.HttpContext.Current.Session[SessionAccessToken] == null)
            {
                TokenSession = AddTokenSession();
            }
            else
            {
                TokenSession = System.Web.HttpContext.Current.Session[SessionAccessToken].ToString();
            }

            return TokenSession;
        }
        /// <summary>
        /// 添加AccessToken缓存
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        private static string AddTokenSession()
        {
            //获取AccessToken
            string AccessToken = GetAccessToken();
            HttpContext.Current.Session[SessionAccessToken] = AccessToken;
            HttpContext.Current.Session.Timeout = 7200;
            return AccessToken;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="AppSecret"></param>
        /// <returns></returns>
        private static string GetAccessToken()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string respText = CommonMethod.WebRequestPostOrGet(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", APPID, APPSECRET), "");
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            string accessToken = respDic["access_token"].ToString();
            return accessToken;
        }
        #endregion

        public static string GetJSTicketSession()
        {
            string JSTicketSession = "";

            if (System.Web.HttpContext.Current.Session[SessionJSTicket] == null)
            {
                JSTicketSession = AddJSTicketSession();
            }
            else
            {
                JSTicketSession = System.Web.HttpContext.Current.Session[SessionJSTicket].ToString();
            }

            return JSTicketSession;
        }

        private static string AddJSTicketSession()
        {
            string JSTicket = GetJSAPI_Ticket();
            HttpContext.Current.Session[SessionJSTicket] = JSTicket;
            HttpContext.Current.Session.Timeout = 7200;
            return JSTicket;
        }

        private static string GetJSAPI_Ticket()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string respText = CommonMethod.WebRequestPostOrGet(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", BasicApi.GetTokenSession()), "");
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            string accessToken = respDic["ticket"].ToString();
            return accessToken;
        }
    } 
}