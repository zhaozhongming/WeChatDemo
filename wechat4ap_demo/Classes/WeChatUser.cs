using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public class WeChatUser
    {
        public static string GetUserName(string id)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string getUserUrl = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
            getUserUrl = string.Format(getUserUrl, BasicApi.GetTokenSession(), id);//获取token、拼凑url
            string respText = CommonMethod.WebRequestGet(getUserUrl);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["nickname"].ToString();
        }

        public static string GetUserOpenID(string code)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string getUserUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";  
            getUserUrl = string.Format(getUserUrl, BasicApi.APPID, BasicApi.APPSECRET, code);
            string respText = CommonMethod.WebRequestGet(getUserUrl);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["openid"].ToString();
        }
    }
}