using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MajordomoProtocol;
using NetMQ;

namespace AMLWorker
{
    public class MyWorkerActor
    {
        public string service;
        public byte id;

        private IWorker _worker;
        public MyWorkerActor(string service, byte id,IWorker worker)
        {
            this.service = service;
            this.id = id;
            this._worker = worker;

        }

        public void Run()
        {
            var t = new Task(() =>
            {

                var g = new MDPWorker("tcp://localhost:5555", $"{service}",
                    new byte[] {(byte) 'W', (byte) id});

                g.HeartbeatDelay = TimeSpan.FromMilliseconds(10000);
                // logging info to be displayed on screen
                g.LogInfoReady += (s, e) => Console.WriteLine("{0}", e.Info);

                // there is no initial reply
                NetMQMessage reply = null;

                bool exit = false;
                while (!exit)
                {
                    // send the reply and wait for a request
                    var request = g.Receive(reply);

                    Console.WriteLine("Received: {0}", request);

                    // was the worker interrupted
                    if (ReferenceEquals(request, null))
                        break;
                    // echo the request
                    reply = _worker.Process(request);
                }
            });
            t.Start();
        }
    }
}
