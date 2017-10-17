using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace Comms
{
    public interface IServiceServer
    {
        event Func<NetMQMessage, NetMQMessage> OnReceived;
    }
}
