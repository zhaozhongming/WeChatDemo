using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace WebJobIoT.Classes
{
    public enum MediaType
    {
        /// <summary>
        /// 图片（image）: 1M，支持JPG格式
        /// </summary>
        image,
        /// <summary>
        /// 语音（voice）：2M，播放长度不超过60s，支持AMR\MP3格式
        /// </summary>
        voice,
        /// <summary>
        /// 视频（video）：10MB，支持MP4格式
        /// </summary>
        video,
        /// <summary>
        /// 缩略图（thumb）：64KB，支持JPG格式
        /// </summary>
        thumb
    }

    public class Message
    {
        public static string SendToAll(string msg)
        {
            string MsgJson = @"{
                        ""filter"":{
                              ""is_to_all"":true
                           },
                        ""text"":{
                        ""content"":""" + msg + @"""
                        },
                        ""msgtype"":""text""}";

            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string sendallUrl = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}";
            sendallUrl = string.Format(sendallUrl, BasicApi.GetAccessToken());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(sendallUrl, MsgJson);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["errcode"].ToString();//返回0发布成功

        }

        public static string SendToOneUser(string msg)
        {
            string MsgJson = @"{
                        ""touser"":""" + "opJjOw2eda3RE4UZ_jca3WOch5BE" + @""",
                        ""msgtype"":""text"",
                        ""text"":{
                        ""content"":""" + msg + @"""
                        }}";

            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string sendallUrl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
            sendallUrl = string.Format(sendallUrl, BasicApi.GetAccessToken());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(sendallUrl, MsgJson);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["errcode"].ToString();//返回0发布成功

        }

    }
}