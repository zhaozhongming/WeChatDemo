using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace wechat4ap_demo.Classes
{
    public class Menu
    {
        public static string SetupMenuJSON()
        {
            string weixin1 = "";
            string openIDUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid="
                                + BasicApi.APPID
                                + "&redirect_uri=https%3A%2F%2Fdev-apdirect.airproducts.com%2Finvoice%3Fmyparam%3Dtest&response_type=code&scope=snsapi_base&state=1#wechat_redirect";
            weixin1 = @" {
                         ""button"":[
                         
                          {
                               ""name"":""公司信息"",
                                ""sub_button"":[
                                {
                                   ""type"":""view"",
                                   ""name"":""主页"",
                                    ""url"":""http://www.xxx.com""
                                },
                                {
                                   ""type"":""view"",
                                   ""name"":""股票"",
                                   ""url"":""http://xxx.com/phoenix.zhtml?c=92444&p=irol-stockquote&_ga=1.186200544.891814539.1431309572""
                                }]
                          },
                          {	
                              ""name"":""快捷方式"",
                                ""sub_button"":[
                                {
                                   ""type"":""view"",
                                   ""name"":""ServicesNow"",
                                    ""url"":""https://xxx/navpage.do""
                                },
                                {
                                   ""type"":""view"",
                                   ""name"":""Wiki"",
                                   ""url"":""http://xxx/""
                                },
                                {
                                   ""type"":""view"",
                                   ""name"":""my order test 2"",
                                   ""url"":""https://xxx=xuchunbo""
                                },
                                {
                                   ""type"":""view"",
                                   ""name"":""TestIntranetDEV"",
                                   ""url"":""" + openIDUrl + @"""
                                },
                                {
                                    ""type"":""view"",
                                   ""name"":""TimeEntry"",
                                    ""url"":""https://sss""
                                }


                                ]
                          },
                          {
                               ""name"":""产品介绍"",
                               ""sub_button"":[
                                {
                                   ""type"":""view"",
                                   ""name"":""invoice"",
                                   ""key"":""https://xxx/invoice""
                                },
                                {
                                    ""type"":""view"",
                                   ""name"":""L2 Test"",
                                    ""url"":""https://xxx/xxx/secure/thankyou.aspx""
                                },
                                {
                                    ""type"":""view"",
                                   ""name"":""APDirect"",
                                    ""url"":""https://xxx.xxx.com/xxx/default.aspx""
                                }]
                           }]
                     }
                    ";

            return weixin1;
        }

        #region 发布菜单
        /// <summary>
        /// 发布菜单
        /// </summary>
        /// <param name="MenuJson">配置的菜单json数据</param>
        /// <param name="AppID">AppID</param>
        /// <param name="AppSecret">AppSecret</param>
        /// <returns>返回0成功否则错误码</returns>
        public static string MenuCreate(string MenuJson)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string setMenuUrl = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";
            setMenuUrl = string.Format(setMenuUrl, BasicApi.GetTokenSession());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(setMenuUrl, MenuJson);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["errcode"].ToString();//返回0发布成功
        }
        #endregion
    }
}