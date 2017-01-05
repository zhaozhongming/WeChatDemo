using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Web.Script.Serialization;

using wechat4ap_demo.Classes;
using wechat4ap_demo.Models;

using SweetCaptcha;
using System.Threading.Tasks;
using System.Net.Http;

namespace wechat4ap_demo.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //IoThub.SendCloudToDeviceMessageAsync(1);

            Response.ContentType = "text/plain";
            if (Request.HttpMethod.ToLower() == "post")
            {
                //回复消息的时候也需要验证消息，这个很多开发者没有注意这个，存在安全隐患  
                //微信中 谁都可以获取信息 所以 关系不大 对于普通用户 但是对于某些涉及到验证信息的开发非常有必要
                if (CheckSignature())
                {
                    //接收消息
                    await Message.ReceiveXml();
                }
                else
                {
                    Response.Write("消息并非来自微信");
                    Response.End();
                }
            }
            else
            {
                Valid();
            }

            return null;
        }

        private bool CheckSignature()
        {
            string signature = Request.QueryString["signature"].ToString();
            string timestamp = Request.QueryString["timestamp"].ToString();
            string nonce = Request.QueryString["nonce"].ToString();
            string[] ArrTmp = { "wechat4ap", timestamp, nonce };
            Array.Sort(ArrTmp); 
            string tmpStr = string.Join("", ArrTmp);

            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();

            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Valid()
        {
            string echoStr = Request.QueryString["echoStr"].ToString();

            if (CheckSignature())
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    Response.Write(echoStr);
                    Response.End();
                }
            }
        }

        public ActionResult LoginusingWeChatConfirm()
        {
            string openIDUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid="
                                + BasicApi.APPID
                                + "&redirect_uri=https%3A%2F%2Fsecure.airproducts.com%2Flogin%2Fregistration.aspx?source=wechat&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect";

            return Redirect(openIDUrl);
        }

        public ActionResult LoginusingWeChatSilently()
        {
            string openIDUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid="
                                + BasicApi.APPID
                                + "&redirect_uri=https%3A%2F%2Fsecure.airproducts.com%2Flogin%2Fregistration.aspx?source=wechat&response_type=code&scope=snsapi_base&state=1#wechat_redirect";

            return Redirect(openIDUrl);
        }



        public ActionResult Public()
        {
            
            return View("Public");
        }


        public ActionResult Help()
        {
            return View("Help");
        }

        public ActionResult Admin()
        {
            return View("Admin");
        }

        public ActionResult Show4s()
        {
            return View("4s");
        }

        public ActionResult tellus()
        {
            return View("tellus");
        }

        public ActionResult AboutUs()
        {
            return View("aboutUs");
        }

        public ActionResult Show4sItem(int id)
        {
            ViewBag.id = id;
            return View("4sItem");
        }

        public ActionResult Func1()
        {
            if (Request.Params["code"] != null)
            {
                ViewBag.OpenID = WeChatUser.GetUserOpenID(Request.Params["code"].ToString());
            }

            if (Request.Params["ReturnUrl"] != null)
            {
                ViewBag.OpenID += Request.Params["ReturnUrl"].ToString();
            }

            List<SAPOrder> olist = SAPProxy.GetMyOrder();

            return View(olist);
        }

        public ActionResult SendToAll(string sendToAllText)
        {
            string MsgJson = @"{
                        ""filter"":{
                              ""is_to_all"":true
                           },
                        ""text"":{
                        ""content"":""" + sendToAllText + @"""
                        },
                        ""msgtype"":""text""}";

            ViewBag.ReturnMessage = Message.SendToAll(MsgJson);
            
            return View("Admin");
        }

        public ActionResult SendToOneUser(string sendToAllText)
        {
            string MsgJson = @"{
                        ""touser"":""" + "opJjOw2eda3RE4UZ_jca3WOch5BE" + @""",
                        ""msgtype"":""text"",
                        ""text"":{
                        ""content"":""" + "This message is only for Chandler!" + @"""
                        }}";

            ViewBag.ReturnMessage = Message.SendToOneUser(MsgJson);

            return View("Admin");
        }

        public ActionResult SendToAllArticle()
        {
            string MsgJson = @"{
                               ""filter"":{
                                  ""is_to_all"":true
                               },
                               ""mpnews"":{
                                  ""media_id"":""fCtIJbvwC3Sq865w0QfL2Y_fe4TLPqeXy9VKQA1s-b-ebEmUCDO_1VUHcSuntVSH""
                               },
                                ""msgtype"":""mpnews""
                            }";

            ViewBag.ReturnMessage = Message.SendToAll(MsgJson);

            return View("Admin");
        }

        public ActionResult UploadMedia()
        {

            ViewBag.ReturnMessage = Message.UploadMedia(Server.MapPath("~/images/534099.jpeg"), BasicApi.GetTokenSession(), MediaType.image).media_id;

            return View("Admin");
        }


        
        //MzmdPCdiYpylbOYILbe2h6aD8ZTJ-xPsGmGX46nBOAZViCshzSPOXHYvf5I8qDJA
        public ActionResult UploadArticle()
        {
            string MsgJson = @"{
               ""articles"": [
		             {
                                    ""thumb_media_id"":""P-E6ofNf-o8voALjPN9r2PKAk4zXbmhuOxR7MqYVNbQNbipbzky-eyBR94fSPP_E"",
                                    ""author"":""xxx"",
			             ""title"":""Happy Day"",
			             ""content_source_url"":""www.airproducts.com"",
			             ""content"":""content"",
			             ""digest"":""digest"",
                                    ""show_cover_pic"":""1""
		             }
               ]
                    }";

            ViewBag.ReturnMessage = Message.UploadArticle(MsgJson);

            return View("Admin");
        }

        public JsonResult GetLocationsJSON()
        {
            UserInfoDA userDA = new UserInfoDA();
            return Json(userDA.GetUserLocation(null));
        }

        public ActionResult Map()
        {

            //test code
            //string na = WeChatUser.GetUserName("gh_36da386fa9cd");

            UserInfoDA userDA = new UserInfoDA();
            //userDA.UpdateUserLocation("test", "test", 2, 2);

            ViewBag.LocationsJSON = userDA.GetUserLocation(null);
            return View("Map");
        }


        public ActionResult TestSetSession()
        {

            Session["S1"] = "It is fromm Session";

            return View("Admin");
        }


        public ActionResult TestReadSession()
        {

            ViewBag.SessionValue = Session["S1"].ToString();

            return View("Admin");
        }

        public ActionResult reCAPTCHA()
        {
            var res = Request["g-recaptcha-response"].ToString();
            ViewBag.ReCaptchaResult = ReCaptcha.Validate(res);

            return View("Public");
        }

        public async Task<ActionResult> Sweet()
        {
            var sweetcaptch = new Classes.SweetCaptcha();

            if (Request["method"] == "check")
            {
                var result = await sweetcaptch.Check(Request["sckey"], Request["scvalue"]);
                return Content(result.ToString());
            }
            else
            {
                var html = await sweetcaptch.GetHtml();
                return Content(html);
            }
        }
    }
}
