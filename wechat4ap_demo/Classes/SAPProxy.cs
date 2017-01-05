using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using wechat4ap_demo.Models;

namespace wechat4ap_demo.Classes
{
    public class SAPProxy
    {
        public static List<SAPOrder> GetMyOrder()
        {
            List<SAPOrder> orders = new List<SAPOrder>();

            string myOrderUrl = "https://int-apdirect.airproducts.com/sap/zodata/ZWECHAT_SRV/SalesOrdersSet?$filter=SoldTo eq '1002636'&sap-client=110&sap-language=EN&sap-user=xub6&sap-password=xuchunbo";
            string myOrderRaw = CommonMethod.WebRequestGet(myOrderUrl);

            
            XmlDocument xmlOrder = new XmlDocument();
            xmlOrder.LoadXml(myOrderRaw);

            System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlOrder.NameTable);

            //Add the namespaces used in books.xml to the XmlNamespaceManager.
            xmlnsManager.AddNamespace("b", "http://www.w3.org/2005/Atom");
            xmlnsManager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            xmlnsManager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            
            XmlNodeList entryList = xmlOrder.GetElementsByTagName("entry");

            foreach (XmlNode entryNode in entryList)
            {
                SAPOrder order = new SAPOrder();
                order.ItmNumber = entryNode.SelectSingleNode("b:content/m:properties/d:ItmNumber", xmlnsManager).InnerText;
                order.Material = entryNode.SelectSingleNode("b:content/m:properties/d:Material", xmlnsManager).InnerText;
                order.ShortText = entryNode.SelectSingleNode("b:content/m:properties/d:ShortText", xmlnsManager).InnerText;
                order.PurchNo = entryNode.SelectSingleNode("b:content/m:properties/d:PurchNo", xmlnsManager).InnerText;
                order.NetValue = entryNode.SelectSingleNode("b:content/m:properties/d:NetValue", xmlnsManager).InnerText;


                order.SdDoc = entryNode.SelectSingleNode("b:content/m:properties/d:SdDoc", xmlnsManager).InnerText;
                order.DocStatus = entryNode.SelectSingleNode("b:content/m:properties/d:DocStatus", xmlnsManager).InnerText;
                order.PurchNoC = entryNode.SelectSingleNode("b:content/m:properties/d:PurchNoC", xmlnsManager).InnerText;
                order.Name = entryNode.SelectSingleNode("b:content/m:properties/d:Name", xmlnsManager).InnerText;
                order.ReqQty = entryNode.SelectSingleNode("b:content/m:properties/d:ReqQty", xmlnsManager).InnerText;
                order.BaseUom = entryNode.SelectSingleNode("b:content/m:properties/d:BaseUom", xmlnsManager).InnerText;

                orders.Add(order);
            }
            return orders;
        }
    }
}