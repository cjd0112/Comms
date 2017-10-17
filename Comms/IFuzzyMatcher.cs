using System;
using System.Collections.Generic;
using System.Text;

namespace Comms
{
    public interface IFuzzyMatcher : ICommsContract
    {
        String Select(String foo);
        Int32 Select2(Int32 foo,String blah);
        String Select3(List<String> p);
    }
}
