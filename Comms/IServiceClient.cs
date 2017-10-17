using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;

namespace Comms
{
    public interface IServiceClient
    {
        NetMQMessage Send(NetMQMessage request);
    }
}
