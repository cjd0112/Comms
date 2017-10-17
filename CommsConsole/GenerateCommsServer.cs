using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NetMQ;

namespace CommsConsole
{
    public class GenerateCommsServer : GeneratorBase
    {
        String outer = @"   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public abstract class _NAME_Server : I_NAME_
    {
        private IServiceServer server;
        protected _NAME_Server(IServiceServer server)
        {
            this.server= server;
            this.server.OnReceived += OnReceived;
        }

        private NetMQMessage OnReceived(NetMQMessage request)
        {
            var ret = new NetMQMessage();
            var selector = request.Pop();
            switch (selector.ConvertToString())
            {_HANDLERS_
                default:
                    throw new Exception($""Unexpected selector - {selector}"");
            }
            return ret;
        }

        _FUNCTIONS_
    }
}";

        private Type type;
        public GenerateCommsServer(Type type, List<Type> googleTypes,string sourceDirectoryName) : base(sourceDirectoryName + "/" + ModuleFuncs.GetClassName(type) + "Server.cs")
        {
            this.type = type;

        }

        public void GenerateFile()
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                methods.Add(GenerateMethod(method));
            }

            List<string> handlers = new List<string>();
            foreach (var method in type.GetMethods())
            {
                handlers.Add(GenerateHandler(method));
            }

            O(outer.Replace("_NAME_", ModuleFuncs.GetClassName(type)).
                Replace("_HANDLERS_",handlers.Aggregate("",(x,y)=>x+"\n"+y)).
                Replace("_FUNCTIONS_", methods.Aggregate("", (x, y) => x + "\n" + y)));

            Close();
        }


        String GenerateMethod(MethodInfo method)
        {
            var access = "public abstract";
            var ret = method.ReturnType.Name;
            var name = method.Name;
            var parameters = method.GetParameters().Select(GenerateSignatureParameter)
                .Select(x => x.Item1 + " " + x.Item2).Aggregate("", (x, y) => x + y + ",");

            if (parameters.Last() == ',')
                parameters = parameters.Substring(0, parameters.Length - 1);

            return $"\t\t{access} {ret} {name}({parameters});\n";
        }

        String GenerateHandler(MethodInfo method)
        {
var s = $@"               case ""{method.Name}"":
                {{
                    SUBHANDLERret.Append({GenerateCallingStub(method)});
                    break;
                }}";

            s = s.Replace("SUBHANDLER", GenerateSubHandler(method));
            return s;
        }

        String GenerateCallingStub(MethodInfo method)
        {
            var name = method.Name;
            var parameters = method.GetParameters().Select(GenerateSignatureParameter)
                .Select(x => x.Item2 ).Aggregate("", (x, y) => x + y + ",");

            if (parameters.Last() == ',')
                parameters = parameters.Substring(0, parameters.Length - 1);

            return $"{name}({parameters})";
        }


        String GenerateSubHandler(MethodInfo method)
        {
            String s = "";
            foreach (var c in method.GetParameters())
            {
                s += $"var {c.Name}Frame = request.Pop();\n";
                if (c.ParameterType == typeof(String))
                    s += $"\t\t\t\t\tvar {c.Name} = {c.Name}Frame.ConvertToString();\n";
                else if (c.ParameterType == typeof(Int32))
                    s += $"\t\t\t\t\tvar {c.Name} = {c.Name}Frame.ConvertToInt32();\n";
                else if (c.ParameterType == typeof(Int64))
                    s += $"\t\t\t\t\tvar {c.Name} = {c.Name}Frame.ConvertToInt64();\n";
                else if (typeof(IList).IsAssignableFrom(c.ParameterType))
                {
                    if (c.ParameterType.GenericTypeArguments[0] == typeof(String))
                    {
 s += $@"                
                    var {c.Name} = new List<String>();
                    var cnt = request.Pop().ConvertToInt32();
                    while (cnt-->0)
                    {{
                        {c.Name}.Add(request.Pop().ConvertToString());
                    }}
";
                    }
                }

                s += "\t\t\t\t\t";

            }
            return s;
        }

      
    }
}
