using System;
using System.Collections.Generic;
using System.Text;

namespace AMLWorker
{
    public class ServiceFactory
    {
        public static Service GetService(ServiceType t,int bucket)
        {
            if (t.Type == "FuzzyMatcher")
                return new FuzzyMatcher(t, bucket);

            throw new Exception($"Could not find service type - {t.Type}");
        }
    }
}
