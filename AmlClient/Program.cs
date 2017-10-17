using System;
using System.Linq;
using System.Runtime.InteropServices;
using AmlClient.AS.Application;
using AMLClient;
using Logger;
using MajordomoProtocol;
using NetMQ;
using StructureMap;

namespace AmlClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Container c = null;
                var reg = new MyRegistry(args.Any() == false ? "appsettings.json" : args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);


                var workers = c.GetInstance<ClientActors>();

                var z = new MDPClientAsync("tcp://localhost:5555", new byte[] { (byte)'C', (byte)'1' });
                z.ReplyReady += Z_ReplyReady;
                z.LogInfoReady += Z_LogInfoReady;


                for (byte i = 0; i < workers.NumNodes; i++)
                {
                    for (int q = 0; q < 100; q++)
                    {
                        var z2 = new NetMQMessage();
                        z2.Append($"this is test message number {q} for {i}");
                        z.Send($"AmlWorker_{i}", z2);
                    }
                }

                Console.ReadLine();
                L.CloseLog();


            }
            catch (Exception e)
            {
                L.Exception(e);
            }
        }

        private static void Z_LogInfoReady(object sender, MDPCommons.MDPLogEventArgs e)
        {
            Console.WriteLine(e.Info);
        }

        private static void Z_ReplyReady(object sender, MDPCommons.MDPReplyEventArgs e)
        {
            var msg = e.Reply.Pop();           
            Console.WriteLine("service name is: " + msg.ConvertToString());

            msg = e.Reply.Pop();

            Console.WriteLine("message is: " + msg.ConvertToString());

        }
    }
}
