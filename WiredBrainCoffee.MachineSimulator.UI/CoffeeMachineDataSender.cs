using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCoffee.EventHub.Sender
{
    public interface ICoffeeMachineDataSender
    {
        Task SendDataAsync(CoffeMachineData data);
    }
    public class CoffeeMachineDataSender: ICoffeeMachineDataSender
    {
        private EventHubClient _eventHubClient;
        public CoffeeMachineDataSender(string eventHubConnectionString)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
        }
        public async Task SendDataAsync(CoffeMachineData data)
        {
            //throw new NotImplementedException();
            var dataAsJson = JsonConvert.SerializeObject(data);
            var eventData = new EventData(Encoding.UTF8.GetBytes(dataAsJson));
            await _eventHubClient.SendAsync(eventData);
        }
    }
}
