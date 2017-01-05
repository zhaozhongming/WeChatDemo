using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;
using System.Text;

namespace wechat4ap_demo.Classes
{
    
    public class IoThub
    {
        private static ServiceClient serviceClient;
        private static string connectionString = "HostName=IoThubZhaoz1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=aIt3EVMl+i12JHZGsrSWzbnV8fYRo5PpEz0W54xWQ5I=";

        public static async Task<string> SendCloudToDeviceMessageAsync(int onf)
        {
            string ret = "success";
            try
            {
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                serviceClient.GetFeedbackReceiver();

                var msg = "";

                switch (onf)
                {
                    case 1:
                        msg = "on";
                        break;
                    case 2:
                        msg = "fancy";
                        break;
                    case 0:
                        msg = "off";
                        break;
                }

                var commandMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(msg));
                commandMessage.Ack = DeliveryAcknowledgement.Full;
                commandMessage.MessageId = "WeChat#1";
                await serviceClient.SendAsync("Raspberry1", commandMessage);
            }
            catch(Exception e)
            {
                ret = e.Message;
            }

            return ret;
        }
    }
}