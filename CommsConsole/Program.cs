using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Comms;
using NetMQ;
using NetMQ.Sockets;
using Xunit;

namespace CommsConsole
{
    class Program
    {
        static void Main(string[] args2)
        {
            var ass = Assembly.GetAssembly(typeof(ICommsContract));
            foreach (var type in ass.GetExportedTypes().Where(x=>typeof(ICommsContract).IsAssignableFrom(x)))
            {
                var z = type.GetMethods();

            }
         }


    }
}
