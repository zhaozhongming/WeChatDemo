using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public static class WeChatHelper
    {

        #region Get请求的简单封装

        /// <summary>发起GET请求</summary>
        /// <param name="url">请求URL</param>
        /// <param name="errmsg">错误信息</param>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        public static string Get(string url, out string errmsg, Dictionary<string, object> parameters)
        {
            errmsg = null;
            var strUrl = new StringBuilder(url);
            if (parameters != null && parameters.Count > 0)
            {
                //拼接参数
                strUrl.Append("?");
                foreach (KeyValuePair<string, object> keyValuePair in parameters)
                {
                    strUrl.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(keyValuePair.Key, Encoding.UTF8),
                        HttpUtility.UrlEncode(keyValuePair.Value.ToString(), Encoding.UTF8));
                }
                strUrl.Remove(strUrl.Length - 1, 1);//移除最后一位多出的“&”
            }
            var request = (HttpWebRequest)WebRequest.Create(strUrl.ToString());
            request.Method = "GET";
            request.Timeout = 10000;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = "请求异常：" + ex.Message;
            }
            return null;
        }

        #endregion

        #region SHA1加密算法

        /// <summary>
        /// SHA1加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string GetSha1Str(string str)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(str);
            HashAlgorithm iSha = new SHA1CryptoServiceProvider();
            strRes = iSha.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }

        #endregion

        /// <summary>
        /// 获取调用JS SDK时所需的access_token
        /// 文档地址：http://mp.weixin.qq.com/wiki/15/54ce45d8d30b6bf6758f68d2e95bc627.html
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            //获取access_token在2小时内有效，由于1天内获取次数有限制（2000次），因此缓存2小时
            //return KK.Cache.MyCache.Instance.Get<string>("wxapi/jssdk/accesstoken", () => {
            string errmsg;
            string apiUrl = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", "wx75ee8ceec612733b", "d4624c36b6795d1d99dcf0547af5443d");
            string responseStr = Get(apiUrl, out errmsg, null);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (responseStr == null)
                {
                    //KK.Core.Log.TraceLogger.Info("获取access_token失败，响应内容为空", null);
                }
                else
                {
                    var dic = serializer.Deserialize<Dictionary<string, object>>(responseStr);
                    if (dic.ContainsKey("access_token"))
                    {
                        return dic["access_token"].ToString();
                    }
                    else
                    {
                        //KK.Core.Log.TraceLogger.Info("获取access_token失败，响应内容：" + responseStr, null);
                    }
                }
                return null;
            //}, 120);
        }

        /// <summary>
        /// 获取调用JS SDK时所需的票据
        /// 文档地址：http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html#.E9.99.84.E5.BD.951-JS-SDK.E4.BD.BF.E7.94.A8.E6.9D.83.E9.99.90.E7.AD.BE.E5.90.8D.E7.AE.97.E6.B3.95
        /// </summary>
        /// <returns></returns>
        public static string GetJsApiTicket()
        {
            //获取的ticket在2小时内有效，由于1天内获取次数有限制（2000次），因此缓存2小时
            //return KK.Cache.MyCache.Instance.Get<string>("wxapi/jssdk/jsapiticket", () => {
            string errmsg;
            string apiUrl = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", GetAccessToken());
            string responseStr = Get(apiUrl, out errmsg, null);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (responseStr == null)
                {
                    //KK.Core.Log.TraceLogger.Info("获取ticket失败", null);
                }
                else
                {
                    var dic = serializer.Deserialize<Dictionary<string, object>>(responseStr);
                if (dic.ContainsKey("ticket"))
                    {
                        return dic["ticket"].ToString();
                    }
                }
                return null;
            //}, 120);
        }

        /// <summary>
        /// JS SDK使用权限签名算法
        /// </summary>
        /// <param name="jsapiTicket">The jsapi_ticket.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Sign(string jsapiTicket, string url)
        {
            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
            string timestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            string str = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapiTicket, nonceStr, timestamp, url);
            string signature = GetSha1Str(str);//SHA1加密
            return new Dictionary<string, string>{
                {"url", url},
                {"jsapi_ticket", jsapiTicket},
                {"noncestr", nonceStr},
                {"timestamp", timestamp},
                {"signature", signature}
            };
        }

    }
}