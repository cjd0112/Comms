   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public abstract class FuzzyMatcherServer : IFuzzyMatcher
    {
        private IServiceServer server;
        protected FuzzyMatcherServer(IServiceServer server)
        {
            this.server= server;
            this.server.OnReceived += OnReceived;
        }

        private NetMQMessage OnReceived(NetMQMessage request)
        {
            var ret = new NetMQMessage();
            var selector = request.Pop();
            switch (selector.ConvertToString())
            {
               case "AddEntry":
                {
                    var entries = Helpers.UnpackMessageList<FuzzyWordEntry>(request,FuzzyWordEntry.Parser.ParseFrom);					
                    var methodResult=AddEntry(entries);
                    ret.Append(Convert.ToInt32(methodResult));
                    break;
                }
                default:
                    throw new Exception($"Unexpected selector - {selector}");
            }
            return ret;
        }

        
		public abstract Boolean AddEntry(List<FuzzyWordEntry> entries);

    }
}