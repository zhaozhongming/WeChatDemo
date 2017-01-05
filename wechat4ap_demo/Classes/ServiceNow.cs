using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace wechat4ap_demo.Classes
{
    public class ServiceNow
    {
        public static string CallService(string url)
        {
            // Create a request for the URL.         
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(url), "Basic",
                new NetworkCredential("appsvc", "AppSvc12@#"));
            request.Method = "GET";
            request.Accept = "application/json";
            request.Credentials = credentialCache;
            //Get the response.
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();


            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();


            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public static string GetUserID(string empID)
        {
            string retJson = CallService("https://airprod" + "uctslab.service-now.com/api/now/v1/table/sys_user?employee_number=" + empID.Substring(3));


            string finalRet = string.Empty;

            dynamic userInfo = JsonConvert.DeserializeObject(retJson);

            finalRet = userInfo.result[0].sys_id;

            return finalRet;
        }

        public static string GetIncident(string empID)
        {
            string userID = GetUserID(empID);

            string retJson = CallService("https://airpro" +    "ductslab.service-now.com/api/now/v1/table/incident?caller_id=" + userID);

            dynamic incidentList = JsonConvert.DeserializeObject(retJson);

            string finalRet = string.Empty;
            foreach (var inci in incidentList.result)
            {
                finalRet += inci.number + ",";
            }
            if (finalRet.EndsWith(","))
                finalRet = finalRet.Substring(0, finalRet.Length - 1);

            return finalRet;
        }
    }
}