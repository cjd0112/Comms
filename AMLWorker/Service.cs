using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comms;
using NetMQ;
using MajordomoProtocol;

namespace AMLWorker
{
    public class Service : IServiceServer
    {
        private ServiceType parent;
        private int bucketId;
        public Service(ServiceType parent, int bucketId)
        {
            this.parent = parent;
            this.bucketId = bucketId;
        }

        public void Run()
        {
            var t = new Task(() =>
            {

                var g = new MDPWorker("tcp://localhost:5555", parent.GetServiceName(bucketId),
                                      new byte[] { (byte)'W', (byte)(bucketId+'A') });

                g.HeartbeatDelay = TimeSpan.FromMilliseconds(10000);
                // logging info to be displayed on screen
                g.LogInfoReady += (s, e) => Console.WriteLine($"{e.Info}", e.Info);

                // there is no initial reply
                NetMQMessage reply = null;

                bool exit = false;
                while (!exit)
                {
                    // send the reply and wait for a request
                    var request = g.Receive(reply);

                    Console.WriteLine($"Received: {request}");

                    // was the worker interrupted
                    if (ReferenceEquals(request, null))
                        break;
                    // echo the request
                    if (OnReceived != null)
                        reply = OnReceived(request);
                    else
                        throw new Exception("OnReceived handler is null - is handler set?");
                }
            });
            t.Start();
        }

        public event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}
