using System;
using AMLClient;
using AS.Logger;
using Logger;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace AmlClient
{
    namespace AS.Application
    {
        public class PubSubObj
        {
            public string hostname { get; set; }
            public int port { get; set; }

        }
        class PubSubServerConfig
        {
            public PubSubObj PubSub { get; set; }
        }
        public class MyRegistry : Registry
        {
            public MyRegistry(string jsonFile)
            {
                Console.WriteLine($"Opening configuration file - {jsonFile}");
                var builder = new ConfigurationBuilder()
                   .AddJsonFile(jsonFile, optional: true, reloadOnChange: true);

                var config = builder.Build();

                LogSource ls;
                Enum.TryParse<LogSource>(config["ApplicationName"], out ls);

                L.InitLogConsole(Console.Out, config["ApplicationName"]);// config["TraceFilePath"], ls);

                L.Trace($"Opening log file");

                L.Trace("Initializing Dependencies");

                var workers = config.Get<ClientActorConfig>();

                For<ClientActors>().Use(workers.ClientActors);

                For<IClient>().Use(() => new AmlClient());

                Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.SingleImplementationsOfInterface();
                });

            }
        }
    }

}
