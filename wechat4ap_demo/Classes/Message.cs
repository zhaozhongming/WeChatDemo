using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;
using wechat4ap_demo.Models;
using System.Threading.Tasks;

namespace wechat4ap_demo.Classes
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
        /// <summary>
        /// 接收微信发送的XML消息并且解析
        /// </summary>
        public static async Task<string> ReceiveXml()
        {
            Stream requestStream = System.Web.HttpContext.Current.Request.InputStream;
            byte[] requestByte = new byte[requestStream.Length];
            requestStream.Read(requestByte, 0, (int)requestStream.Length);
            string requestStr = Encoding.UTF8.GetString(requestByte);
            UserInfoDA userDA = new UserInfoDA();
            bool showMenu = true;

            if (!string.IsNullOrEmpty(requestStr))
            {
                //封装请求类
                XmlDocument requestDocXml = new XmlDocument();
                requestDocXml.LoadXml(requestStr);
                XmlElement rootElement = requestDocXml.DocumentElement;
                WxXmlModel WxXmlModel = new WxXmlModel();
                WxXmlModel.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;
                WxXmlModel.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;
                WxXmlModel.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;
                WxXmlModel.MsgType = rootElement.SelectSingleNode("MsgType").InnerText;
                switch (WxXmlModel.MsgType)
                {
                    case "text"://文本
                        WxXmlModel.Content = rootElement.SelectSingleNode("Content").InnerText;
                        break;
                    case "image"://图片
                        WxXmlModel.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                        break;
                    case "event"://事件
                        WxXmlModel.Event = rootElement.SelectSingleNode("Event").InnerText;
                        if (WxXmlModel.Event == "subscribe" || WxXmlModel.Event == "CLICK")//关注类型
                        {
                            WxXmlModel.EventKey = rootElement.SelectSingleNode("EventKey").InnerText;
                        }
                        //if (WxXmlModel.Event.ToUpper().Equals("LOCATION"))//关注类型
                        //{
                        //    userDA.UpdateUserLocation("Event Test", "Event Test", Convert.ToDecimal(31.229919), Convert.ToDecimal(121.47787));

                        //    showMenu = false;
                        //    WxXmlModel.Location_X = rootElement.SelectSingleNode("Latitude").InnerText;
                        //    WxXmlModel.Location_Y = rootElement.SelectSingleNode("Longitude").InnerText;
                        //    WxXmlModel.Scale = rootElement.SelectSingleNode("Precision").InnerText;
                        //    string userName = WxXmlModel.FromUserName;
                        //    try
                        //    {
                        //        userName = WeChatUser.GetUserName(WxXmlModel.FromUserName);
                        //    }
                        //    catch { }

                        //    userDA.UpdateUserLocation(WxXmlModel.FromUserName, userName, 
                        //        Convert.ToDecimal(WxXmlModel.Location_X), Convert.ToDecimal(WxXmlModel.Location_Y));
                        //}
                        break;
                    default:
                        break;
                }

                //if (showMenu)
                //    Menu.MenuCreate(Menu.SetupMenuJSON());

                return await ResponseXML(WxXmlModel);//回复消息

            }

            return string.Empty;
        }

        /// <summary>
        /// 回复文本
        /// </summary>
        /// <param name="FromUserName">发送给谁(openid)</param>
        /// <param name="ToUserName">来自谁(公众账号ID)</param>
        /// <param name="Content">回复类型文本</param>
        /// <returns>拼凑的XML</returns>
        public static string ReText(string FromUserName, string ToUserName, string Content)
        {
            string XML = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>";//发送给谁(openid)，来自谁(公众账号ID)
            XML += "<CreateTime>" + CommonMethod.DateTimeToInt(DateTime.Now) + "</CreateTime>";//回复时间戳
            XML += "<MsgType><![CDATA[text]]></MsgType>";//回复类型文本
            XML += "<Content><![CDATA[" + Content + "]]></Content><FuncFlag>0</FuncFlag></xml>";//回复内容 FuncFlag设置为1的时候，自动星标刚才接收到的消息，适合活动统计使用
            return XML;
        }

        /// <summary>
        /// 回复单图文
        /// </summary>
        /// <param name="FromUserName">发送给谁(openid)</param>
        /// <param name="ToUserName">来自谁(公众账号ID)</param>
        /// <param name="Title">标题</param>
        /// <param name="Description">详情</param>
        /// <param name="PicUrl">图片地址</param>
        /// <param name="Url">地址</param>
        /// <returns>拼凑的XML</returns>
        public static string ReArticle(string FromUserName, string ToUserName, string Title, string Description, string PicUrl, string Url)
        {
            string XML = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>";//发送给谁(openid)，来自谁(公众账号ID)
            XML += "<CreateTime>" + CommonMethod.DateTimeToInt(DateTime.Now) + "</CreateTime>";//回复时间戳
            XML += "<MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>1</ArticleCount><Articles>";
            XML += "<item><Title><![CDATA[" + Title + "]]></Title><Description><![CDATA[" + Description + "]]></Description><PicUrl><![CDATA[" + PicUrl + "]]></PicUrl>";
            if (Url != null)
                XML += "<Url><![CDATA[" + Url + "]]></Url>";
            XML += "</item>";
            XML += "</Articles><FuncFlag>0</FuncFlag></xml>";
            return XML;
        }

        public static string ReArticle(string FromUserName, string ToUserName, string[] Title, string[] Description, string[] PicUrl, string[] Url)
        {
            string XML = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>";//发送给谁(openid)，来自谁(公众账号ID)
            XML += "<CreateTime>" + CommonMethod.DateTimeToInt(DateTime.Now) + "</CreateTime>";//回复时间戳
            XML += "<MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>5</ArticleCount><Articles>";

            for (int i = 0; i < Title.Count(); i++)
            {
                XML += "<item><Title><![CDATA[" + Title[i] + "]]></Title><Description><![CDATA[" + Description[i] + "]]></Description><PicUrl><![CDATA[" + PicUrl[i] + "]]></PicUrl>";
                if (Url[i] != null)
                    XML += "<Url><![CDATA[" + Url[i] + "]]></Url>";
                XML += "</item>";
            }

            XML += "</Articles></xml>";
            return XML;
        }

        /// <summary>
        /// 多图文回复
        /// </summary>
        /// <param name="FromUserName">发送给谁(openid)</param>
        /// <param name="ToUserName">来自谁(公众账号ID)</param>
        /// <param name="ArticleCount">图文数量</param>
        /// <param name="dtArticle"></param>
        /// <returns></returns>
        public static string ReArticle(string FromUserName, string ToUserName, int ArticleCount, System.Data.DataTable dtArticle)
        {
            string XML = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>";//发送给谁(openid)，来自谁(公众账号ID)
            XML += "<CreateTime>" + CommonMethod.DateTimeToInt(DateTime.Now) + "</CreateTime>";//回复时间戳
            XML += "<MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>" + ArticleCount + "</ArticleCount><Articles>";
            foreach (System.Data.DataRow Item in dtArticle.Rows)
            {
                XML += "<item><Title><![CDATA[" + Item["Title"] + "]]></Title><Description><![CDATA[" + Item["Description"] + "]]></Description><PicUrl><![CDATA[" + Item["PicUrl"] + "]]></PicUrl><Url><![CDATA[" + Item["Url"] + "]]></Url></item>";
            }
            XML += "</Articles><FuncFlag>0</FuncFlag></xml>";
            return XML;
        }

        public static async Task<string> GetTextAsync(string FromUserName, string ToUserName, string Content)
        {
            //sohovan.com.common.CommonMethod.WriteTxt(Content);//接收的文本消息
            string XML = "";

            if (Content.ToLower().StartsWith("emp"))
            {
                string snMsg = ServiceNow.GetIncident(Content);
                XML = ReText(FromUserName, ToUserName, "sorry, you just can't.....");
                return XML;
            }
            switch (Content.ToLower())
            {
                case "1":
                    await IoThub.SendCloudToDeviceMessageAsync(1);
                    break;
                case "0":
                    await IoThub.SendCloudToDeviceMessageAsync(0);
                    break;
                case "2":
                    await IoThub.SendCloudToDeviceMessageAsync(2);
                    break;
                default:
                    string docID = MSDSHelper.GetDocumentID(Content);
                    switch (docID)
                    {
                        case "err":
                            XML = ReText(FromUserName, ToUserName, "对不起，查询MSDS出错。");
                            break;
                        case "":
                            XML = ReText(FromUserName, ToUserName, "对不起，我不明白这是什么意思。");
                            break;
                        default:
                            string desc = "The Safety Data Sheets (SDS) on airproducts.com are the latest publicly-available versions for our products. When you purchase a product from Air Products, your company will be mailed an SDS with the company identification, language, emergency telephone number, and transportation information. The SDSs from this website are not meant to replace copies automatically distributed from our corporate SDS distribution system.";
                                                
                            XML = ReArticle(FromUserName, ToUserName, Content, desc, "http://wechat4ap.azurewebsites.net/images/534099.jpeg", "https://apdirect.airproducts.com/MSDS/DisplayPDF?documentID=" + docID);
                            break;

                    }
                    break;
            }
            return XML;
        }

        public static string GetText(string FromUserName, string ToUserName, string Content)
        {
            //sohovan.com.common.CommonMethod.WriteTxt(Content);//接收的文本消息
            string XML = "";

            if (Content.ToLower().StartsWith("emp"))
            {
                string snMsg = ServiceNow.GetIncident(Content);
                XML = ReText(FromUserName, ToUserName, "sorry, you just can't.....");
                return XML;
            }
            switch (Content.ToLower())
            {
                case "stock":
                case "st":
                case "股票":
                case "gupiao":
                    string stock = GetStockPrice();
                    XML = ReText(FromUserName, ToUserName, stock);
                    break;
                case "myorder":
                    List<SAPOrder> olist = SAPProxy.GetMyOrder();
                    var json = new JavaScriptSerializer().Serialize(olist);
                    XML = ReText(FromUserName, ToUserName, json);
                    break;
                case "pm":
                    var pm25 = PM25DA.GetPM25();
                    XML = ReText(FromUserName, ToUserName, pm25);
                    break;
                default:
                    string docID = MSDSHelper.GetDocumentID(Content);
                    switch (docID)
                    {
                        case "err":
                            XML = ReText(FromUserName, ToUserName, "对不起，查询MSDS出错。");
                            break;
                        case "":
                            XML = ReText(FromUserName, ToUserName, "对不起，我不明白这是什么意思。");
                            break;
                        default:
                            string desc = "The Safety Data Sheets (SDS) on airproducts.com are the latest publicly-available versions for our products. When you purchase a product from Air Products, your company will be mailed an SDS with the company identification, language, emergency telephone number, and transportation information. The SDSs from this website are not meant to replace copies automatically distributed from our corporate SDS distribution system.";

                            XML = ReArticle(FromUserName, ToUserName, Content, desc, "http://wechat4ap.azurewebsites.net/images/534099.jpeg", "https://apdirect.airproducts.com/MSDS/DisplayPDF?documentID=" + docID);
                            break;

                    }
                    break;
            }
            return XML;
        }
        private static async Task<string> ResponseXML(WxXmlModel WxXmlModel)
        {
            string XML = "";
            switch (WxXmlModel.MsgType)
            {
                case "text"://文本回复
                    if (WxXmlModel.Content.ToLower().Equals("1")
                          || WxXmlModel.Content.ToLower().Equals("2")
                          || WxXmlModel.Content.ToLower().Equals("0"))
                    {
                        XML = await GetTextAsync(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.Content);
                        return "";
                    }
                    else if (WxXmlModel.Content.StartsWith("["))
                    {
                        try
                        { 
                            XML = await CognitiveService.GetSentiment(WxXmlModel.Content.Substring(1, (WxXmlModel.Content.Length - 2)));
                        }
                        catch(Exception e)
                        {
                            XML = e.Message;
                        }
                        XML = ReText(WxXmlModel.FromUserName, WxXmlModel.ToUserName, XML);
                    }
                    else
                        XML = GetText(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.Content);
                    break;
                case "event"://文本回复
                    if (WxXmlModel.Event == "subscribe")//关注类型
                    {
                        string desc = "1.	点击查看4S文化。                                             "
                                    + "2.	发送消息”st”，“Stock“或者”股票“甚至”gupiao“，大小写不敏感，会返回当前APD股价信息。                                             "
                                        + "3.	整合了MSDS查询功能，发送公司任意产品的名字，比如”oxygen“，”hele“，” PEP 100 Promoter“，会得到产品具体信息PDF文档。";

                        XML = ReArticle(WxXmlModel.FromUserName, WxXmlModel.ToUserName, "欢迎关注！", desc, "http://wechat4ap.azurewebsites.net/images/4swelcome3.png", "http://wechat4ap.azurewebsites.net/Home/show4s");
                    }
                    //if (WxXmlModel.Event.ToUpper().Equals("LOCATION"))//关注类型
                    //{
                    //    XML = ReText(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.Location_X);
                    //}

                    if (WxXmlModel.Event == "CLICK" && WxXmlModel.EventKey == "4s")
                    {
                        string[] Titles = new string[] { "Great Job in the Brazil TM implementation project - Vijaya Keerthi Penmetsa"
                                                            ,"Great Effort Supporting Machinery Commissioning - Kevin Mc Colgan"
                                                            ,"Quickly quench a flame at Qingpu PHG - Peng Jet"
                                                            ,"工廠跳車,隨call隨到- Andersen Chaur Yih Lu"
                                                            ,"15 Years of Service -Annie Ramsey"};

                        string[] Descriptions = new string[] { "Keerthi did an excellent job in the Brazil TM implementation project. Keerthi is very quick in fixing defects and addressing issues. There is a spec change after I see the live credit card feed from the bank, some unexpected transactions are sent in the statement, which we do not want to import into SAP. Keerthi made changes to the spec changes very quickly. It's impossible for the project team to deliver the project successfully without Keerthi's help! Great Job!"
                                                            ,"Thank you for leading, communicating all machinery commissioning issues at BRS"
                                                            ,"On 29th July Peng Jet worked a night shift at Shanghai Qingpu PHG site, he found a flame at vent point(H2 was ignited by static eletricity), he report to manager and quench a fire by water quickly, which eliminated a big safety risk and no any influence to both AP and customer. As an operator leader, in past years he always work diligently and actively, and already found and resolved several big issue and risk. Appreciate his significant contribution "
                                                            ,"工廠跳車,隨call隨到- Andersen Chaur Yih Lu "
                                                            ,"15 Years of Service -Annie Ramsey"};

                        string[] PicUrls = new string[] { "http://wechat4ap.azurewebsites.net/images/us1.jpg"
                                                            ,"http://wechat4ap.azurewebsites.net/images/us2.jpg"
                                                            ,"http://wechat4ap.azurewebsites.net/images/us3.jpg"
                                                            ,"http://wechat4ap.azurewebsites.net/images/us4.jpg"
                                                            ,"http://wechat4ap.azurewebsites.net/images/us5.jpg"};

                        string[] Urls = new string[] { "http://wechat4ap.azurewebsites.net/Home/Show4sItem/0"
                                                            ,"http://wechat4ap.azurewebsites.net/Home/Show4sItem/1"
                                                            ,"http://wechat4ap.azurewebsites.net/Home/Show4sItem/2"
                                                            ,"http://wechat4ap.azurewebsites.net/Home/Show4sItem/3"
                                                            ,"http://wechat4ap.azurewebsites.net/Home/Show4sItem/4"};

                        XML = ReArticle(WxXmlModel.FromUserName, WxXmlModel.ToUserName, Titles, Descriptions, PicUrls , Urls);
                        
                    }
                    break;
                default://默认回复
                    break;
            }
           
            return XML;
        }

        private static string GetStockPrice()
        {
            string price = "can NOT get the stock price at this moment";

            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                string xmlString = webClient.DownloadString("http://finance.yahoo.com/d/quotes.csv?s=" + "APD" + "&f=sl1d1t1c1ohgv&e=.csv");
                string[] stData = xmlString.Split(',');
                price = xmlString;
                //price = string.Format("股票代码：{0}/r/n股价：{1}/r/n日期：{2}/r/n涨跌：{3}",stData[0].ToString(), stData[1].ToString(), stData[2] + " " + stData[3], stData[4].ToString());
            }
            catch { }

            return price;
        }

        public static string SendToAll(string MsgJson)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string sendallUrl = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}";
            sendallUrl = string.Format(sendallUrl, BasicApi.GetTokenSession());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(sendallUrl, MsgJson);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["errcode"].ToString();//返回0发布成功
        
        }

        public static string SendToOneUser(string MsgJson)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string sendallUrl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
            sendallUrl = string.Format(sendallUrl, BasicApi.GetTokenSession());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(sendallUrl, MsgJson);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["errcode"].ToString();//返回0发布成功

        }


        //return media_id: 3HOFiBmTtW4lpEO-9JljTW3E1FLusc9hV2MdK2fERJuoxgZ_oLqtz4joeiaWmqlk 
        public static UpLoadInfo UploadMedia(string filepath, string token, MediaType mt)
        {
            using (WebClient client = new WebClient())
            {
                byte[] b = client.UploadFile(string.Format("http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", token, mt.ToString()), filepath);//调用接口上传文件
                string retdata = Encoding.Default.GetString(b);//获取返回值
                if (retdata.Contains("media_id"))//判断返回值是否包含media_id，包含则说明上传成功，然后将返回的json字符串转换成json
                {
                    return JsonConvert.DeserializeObject<UpLoadInfo>(retdata);
                }
                else
                {
                    return null;
                }
            }
        }

        public static string UploadArticle(string articleMsg)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string uploadArticleUrl = "https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token={0}";
            uploadArticleUrl = string.Format(uploadArticleUrl, BasicApi.GetTokenSession());//获取token、拼凑url
            string respText = CommonMethod.WebRequestPostOrGet(uploadArticleUrl, articleMsg);
            Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);
            return respDic["media_id"].ToString();//返回0发布成功
        }

        

    }
}