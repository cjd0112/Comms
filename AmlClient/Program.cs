using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AmlClient.AS.Application;
using Comms;
using Logger;
using MajordomoProtocol;
using NetMQ;
using StructureMap;

namespace AmlClient
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                Container c = null;
                var reg = new MyRegistry(args.Any() == false ? "appsettings.json" : args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);

                var clientFactory = new ClientFactory(c);

                clientFactory.Run();
              
                Console.ReadLine();
                L.CloseLog();


            }
            catch (Exception e)
            {
                L.Exception(e);
            }
        }

      
    }
}
