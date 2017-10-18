using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using Comms;
using MajordomoProtocol;
using NetMQ;
using StructureMap;

namespace AmlClient
{
    public class ClientFactory : IClientProxy
    {
        private MDPClientAsync mainClient;

        Dictionary<string,ServiceClient> serviceQueue = new Dictionary<string, ServiceClient>();

        ConcurrentQueue<Tuple<String,NetMQMessage>> myQueue = new ConcurrentQueue<Tuple<String,NetMQMessage>>();

        private Container container;
        public ClientFactory(Container c)
        {
            mainClient = new MDPClientAsync("tcp://localhost:5555", new byte[] { (byte)'C', (byte)'1' });
            mainClient.ReplyReady += Z_ReplyReady;
            mainClient.LogInfoReady += Z_LogInfoReady;

            var pp = new NetMQMessage();
            pp.Append("list");

            // get a list of the services that we support
            mainClient.Send("mmi.service", pp);

            this.container = c;

        }

        private void Z_LogInfoReady(object sender, MDPCommons.MDPLogEventArgs e)
        {
            Console.WriteLine(e.Info);
        }

        private void Z_ReplyReady(object sender, MDPCommons.MDPReplyEventArgs e)
        {
            var msg = e.Reply.Pop();
            var serviceName = msg.ConvertToString();
            Console.WriteLine("service name is: " + msg.ConvertToString());
            if (serviceName == "mmi.service")
            {
                CreateServiceClients(e.Reply.Pop().ConvertToString().Split(new char[] { ',' }));
                Run();
            }
            else
            {
                if (serviceQueue.ContainsKey(serviceName) == false)
                    throw new Exception($"Service response found with unexpected name - {serviceName}");
                serviceQueue[serviceName].OnResponse(e.Reply);

                Console.WriteLine("message is: " + msg.ConvertToString());
            }
        }

        void CreateServiceClients(IEnumerable<String> serviceNames)
        {
            foreach (var c in serviceNames)
            {
                try
                {
                    Console.WriteLine($"Loading service- {c}");
                    var interfaceName = c.Substring(0, c.IndexOf("_") - 2);

                    Type interfaceType = null;
                    foreach (var g in Assembly.GetAssembly(typeof(ICommsContract)).GetExportedTypes())
                    {
                        if (g.IsInterface && typeof(ICommsContract).IsAssignableFrom(g) && g.Name == interfaceName)
                        {
                            interfaceType = g;
                        }
                    }

                    var commsAssembly = Assembly.GetAssembly(typeof(ICommsContract));

                    var str = $"Comms.{interfaceName.Substring(1)}Client,{commsAssembly.FullName}";

                    var clientType = System.Type.GetType(str);

                    serviceQueue[c] = container.With(typeof(string), c).With(typeof(IClientProxy), this).GetInstance<ServiceClient>();

                    // hook up the underlying

                    container.With(typeof(IServiceClient), serviceQueue[c]).GetInstance(clientType);


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception loading our client service");
                    Console.WriteLine(e);
                    Console.WriteLine("Continuing with other service initiation ...");
                }
            }

            
        }

        private bool finished = false;

        public void Run()
        {
            var t = new Task(() =>
            {

                while (!finished)
                {
                    Tuple<string, NetMQMessage> msg = null;
                    if (myQueue.TryDequeue(out msg))
                    {
                        mainClient.Send(msg.Item1, msg.Item2);
                    }
                }
            });
            t.Start();

        }


        public void SendMessage(String serviceName,NetMQMessage msg)
        {
            if (serviceQueue.ContainsKey(serviceName) == false)
            {
                throw new Exception($"cannot find serviceQueue for {serviceName}");
            }
            myQueue.Enqueue(new Tuple<string,NetMQMessage>(serviceName,msg));
        }
    }
}
