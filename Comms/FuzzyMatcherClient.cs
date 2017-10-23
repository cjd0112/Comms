   
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
        }

        

		public Boolean AddEntry(List<FuzzyWordEntry> entries)
		{
			var msg = new NetMQMessage();
			msg.Append("AddEntry");
			Helpers.PackMessageList<FuzzyWordEntry>(msg,entries);
			var ret = client.Send(msg);
			return ret.First.ConvertToInt32() >0 ? true:false;
		}
    }
}