using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace AMLWorker
{
    public class FuzzyMatcher : Service
    {
        public FuzzyMatcher(ServiceType parent, int bucketId) : base(parent, bucketId)
        {

        }

        protected override NetMQMessage ProcessRequest(NetMQMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
