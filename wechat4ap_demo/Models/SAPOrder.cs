using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wechat4ap_demo.Models
{
    public class SAPOrder
    {
        public string ItmNumber { get; set; }

        public string SdDoc { get; set; }

        public string DocStatus { get; set; }

        public string PurchNoC { get; set; }

        public string Name { get; set; }

        public string ReqQty { get; set; }

        public string BaseUom { get; set; }

        

        public string Material { get; set; }

        public string ShortText { get; set; }

        public string PurchNo { get; set; }

        public string NetValue { get; set; }

        public DateTime CreationDate { get; set; }
    }
}