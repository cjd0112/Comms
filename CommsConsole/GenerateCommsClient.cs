using System;
using System.Collections.Generic;
using System.Text;

namespace CommsConsole
{
    public class GenerateCommsClient : GeneratorBase
    {
        public GenerateCommsClient(string fileName, string sourceDirectoryName) : base(fileName, sourceDirectoryName)
        {
            O($@"using System;
using System.Text;
using System.Collections.Generic;");

        }



    }
}
