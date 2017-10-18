   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public class FuzzyMatcherClient : IFuzzyMatcher
    {
        private IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
            this.client.SetUnderlying(this);
        }

        

		public String Select(String foo)
		{
			var msg = new NetMQMessage();
			msg.Append("Select");
			msg.Append(foo);
			var ret = client.Send(msg);
			return ret.First.ConvertToString();
		}

		public Int32 Select2(Int32 foo,String blah)
		{
			var msg = new NetMQMessage();
			msg.Append("Select2");
			msg.Append(foo);
			msg.Append(blah);
			var ret = client.Send(msg);
			return ret.First.ConvertToInt32();
		}

		public String Select3(List<String> p)
		{
			var msg = new NetMQMessage();
			msg.Append("Select3");
			msg.Append(p.Count);p.Do(x => msg.Append(x));
			var ret = client.Send(msg);
			return ret.First.ConvertToString();
		}
    }
}