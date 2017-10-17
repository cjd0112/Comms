using NetMQ;

namespace AmlClient
{
    public interface IClient
    {
        NetMQMessage Request(NetMQMessage m);
    }
}