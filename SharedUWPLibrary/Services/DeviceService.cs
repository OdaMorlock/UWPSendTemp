﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedUWPLibrary.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using MAD = Microsoft.Azure.Devices;

namespace SharedUWPLibrary.Services
{
    public static  class DeviceService
    {

        private static readonly Random rnd = new Random();

        public static async Task SendMessageAsync(DeviceClient deviceClient)
        {
            while (true)
            {
                var data = new TemperatureModel
                {
                    Temperature = rnd.Next(20, 30),
                    Humidity = rnd.Next(40, 60)
                };

                //JSON = {"temperature": 20, "humidity":44
                var json = JsonConvert.SerializeObject(data);

                var payload = new Message(Encoding.UTF8.GetBytes(json));
                await deviceClient.SendEventAsync(payload);

                Console.WriteLine($"Message sent: {json}");
                await Task.Delay(60 * 1000);
            }
        }

        public static async Task ReceiveMessageAsync(DeviceClient deviceClient)
        {
            while (true)
            {
                var payload = await deviceClient.ReceiveAsync();

                if (payload == null)
                    continue;


                Console.WriteLine($"Message Received: {Encoding.UTF8.GetString(payload.GetBytes())}");


                await deviceClient.CompleteAsync(payload);
            }
        }

        public static async Task SendMessageToDeviceAsync(MAD.ServiceClient serviceClient, string targetDeviceID, string message)
        {
            var payload = new MAD.Message(Encoding.UTF8.GetBytes(message));
            await serviceClient.SendAsync(targetDeviceID, payload);
        }


    }
}