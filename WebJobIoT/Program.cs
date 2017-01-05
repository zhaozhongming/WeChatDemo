using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using Microsoft.WindowsAzure.Management.WebSites;
using Microsoft.WindowsAzure;

namespace WebJobIoT
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static string connectionString = "HostName=IoThubZhaoz1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=aIt3EVMl+i12JHZGsrSWzbnV8fYRo5PpEz0W54xWQ5I=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            //var host = new JobHost();
            //// The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
                
                if (data.IndexOf("alarm") > -1)
                    WebJobIoT.Classes.Message.SendToOneUser("catch you!");
            }
        }

        private static void GetWebSpace()
        {
            var credentials = GetCredentials(/*using certificate*/);

            using (var client = new WebSiteManagementClient(credentials))
            {
                foreach(var space in client.WebSpaces.List())
                {
                    Console.WriteLine(space.Name);
                }
            }
        }

        private async void UpdateAppSetting(string mySetting, string newValue)
        {
            var credentials = GetCredentials(/*using certificate*/);

            using (var client = new WebSiteManagementClient(credentials))
            {
                var currentConfig = await client.WebSites.GetConfigurationAsync("",
                                                                                "wechat4ap");
                var newConfig = new WebSiteUpdateConfigurationParameters
                {
                    ConnectionStrings = null,
                    DefaultDocuments = null,
                    HandlerMappings = null,
                    Metadata = null,
                    AppSettings = currentConfig.AppSettings
                };
                newConfig.AppSettings[mySetting] = newValue;
                await client.WebSites.UpdateConfigurationAsync("", "wechat4ap",
                                                               newConfig);
            }
        }

        private static SubscriptionCloudCredentials GetCredentials()
        {
            return new CertificateCloudCredentials("67388557-6309-41da-830c-5b9794fc319a", new System.Security.Cryptography.X509Certificates.X509Certificate2("MIIKJAIBAzCCCeQGCSqGSIb3DQEHAaCCCdUEggnRMIIJzTCCBe4GCSqGSIb3DQEHAaCCBd8EggXbMIIF1zCCBdMGCyqGSIb3DQEMCgECoIIE7jCCBOowHAYKKoZIhvcNAQwBAzAOBAi5oJXPRMYjKQICB9AEggTIf4Tk1N7XtOx3Jmt"));
        }
    }
}
