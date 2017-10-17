using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace AMLWorker
{
    public interface IWorker
    {
        NetMQMessage Process(NetMQMessage p);
    }
}
