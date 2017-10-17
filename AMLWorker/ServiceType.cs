using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using StructureMap;

namespace AMLWorker
{
    public class ServiceType
    {
        private ServiceConfig config;
        public List<Service> underlying = new List<Service>();

        public String Type => config.Type;

        private IContainer container;
        public ServiceType(IContainer container,ServiceConfig config)
        {
            this.config = config;
            this.container = container;

            var t = System.Type.GetType(config.Type);

        }

        public String GetServiceName(int bucket)
        {
            return $"{Type}_{bucket}";
        }

        public void Run()
        {
            for (int i = config.BucketStart; i < config.BucketStart + config.BucketCount; i++)
            {
                var q = container
                    .With(typeof(ServiceType), this)
                    .With(typeof(int), i)
                    .GetInstance(System.Type.GetType(config.Type));
                underlying.Add((Service)q);
                underlying.Last().Run();

            }
        }
    }
}
