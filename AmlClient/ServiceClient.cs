using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comms;
using NetMQ;

namespace AmlClient
{
    public class ServiceClient : IServiceClient
    {
        private IClientProxy proxy;
        private string serviceName;
        private ConcurrentQueue<NetMQMessage> myQueue = new ConcurrentQueue<NetMQMessage>();

        public ICommsContract Underlying;

        public ServiceClient(string serviceName,IClientProxy proxy)
        {
            this.proxy = proxy;
            this.serviceName = serviceName;
        }

        public void SetUnderlying(ICommsContract contract)
        {
            Underlying = contract;
        }

        public void OnResponse(NetMQMessage msg)
        {
            myQueue.Enqueue(msg);
        }

        public NetMQMessage Send(NetMQMessage request)
        {
            proxy.SendMessage(serviceName, request);
            NetMQMessage msg = null;
            while (msg == null)
            {
                myQueue.TryDequeue(out msg);
            }
            return msg;
        }
    }
}
