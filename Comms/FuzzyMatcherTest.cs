using System;
using System.Runtime.InteropServices;
using NetMQ;

namespace Comms
{

    public abstract class FuzzyMatcherServer : IFuzzyMatcher
    {
        protected FuzzyMatcherServer(IServiceServer server)
        {
            server.OnReceived += OnReceived;
        }

        NetMQMessage OnReceived(NetMQMessage msg)
        {
            var ret = new NetMQMessage();
            switch (msg.Pop().ToString())
            {
                case "Select":
                {
                    ret.Append(Select(msg.Pop().ToString()));
                    
                }
                break;
            }
            return ret;
        }

        public abstract string Select(string foo);
    }
    public class FuzzyMatcherClient: IFuzzyMatcher
    {
        private IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
        }
        public string Select(string foo)
        {
            var z = new NetMQMessage();
            z.Append("Select");
            z.Append(foo);
            var ret = client.Send(z);
            return ret.First.ConvertToString();
        }

    }
}
