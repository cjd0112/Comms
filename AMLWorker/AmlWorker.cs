using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace AMLWorker
{
    public class AmlWorker : IWorker
    {
        public NetMQMessage Process(NetMQMessage p)
        {
            return p;
        }
    }
}
