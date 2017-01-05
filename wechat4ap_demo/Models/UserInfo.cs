using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wechat4ap_demo.Models
{
    public class UserInfo
    {
        /// <summary>
        /// 消息发送方微信号
        /// </summary>
        public string UserId { get; set; }

        public string UserName { get; set; }
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Location_X { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Location_Y { get; set; }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public string Scale { get; set; }
    }
}