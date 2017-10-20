   
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
               case "Select":
                {
                    var fooFrame = request.Pop();
					var foo = fooFrame.ConvertToString();
					ret.Append(Select(foo));
                    break;
                }
               case "Select2":
                {
                    var fooFrame = request.Pop();
					var foo = fooFrame.ConvertToInt32();
					var blahFrame = request.Pop();
					var blah = blahFrame.ConvertToString();
					ret.Append(Select2(foo,blah));
                    break;
                }
               case "Select3":
                {
                    var pFrame = request.Pop();
                
                    var p = new List<String>();
                    var cnt = request.Pop().ConvertToInt32();
                    while (cnt-->0)
                    {
                        p.Add(request.Pop().ConvertToString());
                    }
					ret.Append(Select3(p));
                    break;
                }                default:
                    throw new Exception($"Unexpected selector - {selector}");
            }
            return ret;
        }

        
		public abstract String Select(String foo);

		public abstract Int32 Select2(Int32 foo,String blah);

		public abstract String Select3(List<String> p);

		public abstract Boolean AddEntry(FuzzyWordEntries entries);

    }
}