using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NetMQ;
using Shared;

namespace CommsConsole
{

    /*
        using System;
        using System.Runtime.InteropServices;
        using NetMQ;

namespace Comms
{
     *  
     *  public class FuzzyMatcherClient: IFuzzyMatcher
    {
        private IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
        }
        public string Select(string foo)
        {
            var z = new NetMQMessage();
            z.Append("Select");
            z.Append(foo);
            var ret = client.Send(z);
            return ret.First.ConvertToString();
        }
   }
    */


    public class GenerateCommsClient : GeneratorBase
    {
String outer = @"   
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetMQ;
using Shared;

namespace Comms
{
    public class FuzzyMatcherClient : IFuzzyMatcher
    {
        private IServiceClient client;
        public FuzzyMatcherClient(IServiceClient client)
        {
            this.client = client;
        }

        FUNCTIONS
    }
}";

        private Type type;

        public GenerateCommsClient(Type type, List<Type> googleTypes, string sourceDirectoryName) : base(
            sourceDirectoryName + "/" + ModuleFuncs.GetClassName(type) + "Client.cs")
        {
            this.type = type;

            var z = new NetMQMessage();

            List<String> z2 = new List<string>();

            z2.Do(x => z.Append(x));
        }

        public void GenerateFile()
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                methods.Add(GenerateMethod(method));

                
            }
            O(outer.Replace("FUNCTIONS", methods.Aggregate("", (x, y) => x + "\n\n" + y)));

            Close();
        }


        /*
            var z = new NetMQMessage();
            z.Append("Select");
            z.Append(foo);
            var ret = client.Send(z);
            return ret.First.ConvertToString();
        */

        String GenerateMethod(MethodInfo method)
        {
            var m = GenerateSignature(method);
            m += "\t\t{\n";
            m += "\t\t\tvar msg = new NetMQMessage();\n";
            m += $"\t\t\tmsg.Append(\"{method.Name}\");\n";

            foreach (var c in method.GetParameters())
            {
                m += $"\t\t\t{GenerateParameter(c)};\n";
            }



            m += $"\t\t\tvar ret = client.Send(msg);\n";
            m += $"\t\t\treturn {GenerateReturn(method.ReturnType)};\n";
            m += "\t\t}";


            return m;
        }

        String GenerateSignature(MethodInfo method)
        {
            var access = "public";
            var ret = method.ReturnType.Name;
            var name = method.Name;
            var parameters = method.GetParameters().Select(GenerateSignatureParameter)
                .Select(x => x.Item1 + " " + x.Item2).Aggregate("", (x, y) => x + y + ",");

            if (parameters.Last() == ',')
                parameters = parameters.Substring(0, parameters.Length - 1);

            return $"\t\t{access} {ret} {name}({parameters})\n";
        }


        Tuple<string, string> GenerateSignatureParameter(ParameterInfo pi)
        {
            if (pi.ParameterType == typeof(String) || pi.ParameterType == typeof(Int32))
            {
                return new Tuple<string, string>(pi.ParameterType.Name, pi.Name);
            }
            else if (typeof(IList).IsAssignableFrom(pi.ParameterType))
            {
                return new Tuple<string, string>("List<" + pi.ParameterType.GenericTypeArguments[0].Name + ">",pi.Name);
            }
            return new Tuple<string, string>("", "");

        }

        String GenerateParameter(ParameterInfo pi)
        {
            if (pi.ParameterType == typeof(String) || pi.ParameterType == typeof(Int32))
            {
                return $"msg.Append({pi.Name})";
            }
            else if (typeof(IList).IsAssignableFrom(pi.ParameterType))
            {
                return $"{pi.Name}.Do(x => msg.Append(x))";
            }
            return "";
        }

        String GenerateReturn(Type returnType)
        {
            var z = new NetMQMessage();
            if (returnType == typeof(String))
            {
                return $"ret.First.ConvertToString()";
            }
            else if (returnType == typeof(Int32))
            {
                return $"ret.First.ConvertToInt32()";
            }
            return "";
        }

    }
}
