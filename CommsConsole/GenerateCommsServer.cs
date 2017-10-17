using System;
using System.Collections.Generic;
using System.Text;

namespace CommsConsole
{
    public class GenerateCommsServer : GeneratorBase
    {
        public GenerateCommsServer(Type type, List<Type> googleTypes,string sourceDirectoryName) : base(sourceDirectoryName + "/" + ModuleFuncs.GetClassName(type) + "Server.cs")
        {

        }

        public void GenerateFile()
        {
            
        }
    }
}
