using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace wechat4ap_demo.Classes
{
    public class MSDSHelper
    {
        public static string GetDocumentID(string productName)
        {
            string documentId = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/MSDS.csv"));
                string line;
                string[] row = new string[2];
                while ((line = sr.ReadLine()) != null)
                {
                    row = line.Split(',');
                    if (row[1].ToLower().Equals(productName.ToLower()))
                    {
                        documentId = row[0].ToString();
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                documentId = "err";
            }

            return documentId;
        }
    }
}