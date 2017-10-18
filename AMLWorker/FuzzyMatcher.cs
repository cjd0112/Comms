using System;
using System.Collections.Generic;
using System.Text;
using Comms;

namespace AMLWorker
{
    public class FuzzyMatcher : FuzzyMatcherServer
    {
        public FuzzyMatcher(IServiceServer server) : base(server)
        {
        }

        public override string Select(string foo)
        {
            return foo + "SELECT";
        }

        public override int Select2(int foo, string blah)
        {
            return 234;
        }

        public override string Select3(List<string> p)
        {
            return "blalalal";
        }
    }
}
