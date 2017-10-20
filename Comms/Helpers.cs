using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Protobuf;
using NetMQ;

namespace Comms
{
    public class Helpers
    {
        public static List<T> UnpackMessageList<T>(NetMQMessage msg,Func<byte[],T> parseObject)
        {
            var cnt = msg.FrameCount;
            List<T> ret = new List<T>();
            while (cnt--> 0)
            {
                ret.Add(parseObject(msg.Pop().Buffer));
            }
            return ret;
        }
    }
}
