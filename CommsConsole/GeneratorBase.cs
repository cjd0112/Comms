using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CommsConsole
{
    public class GeneratorBase
    {
        private StreamWriter sw;
        public GeneratorBase(String fileName)
        {
            sw = new StreamWriter(new FileStream(fileName, FileMode.Create));
        }

        public void O(String s)
        {
            sw.Write(s);
        }

        public void L(String s)
        {
            sw.WriteLine(s);
        }

        public void Close()
        {
            sw.Flush();
            sw.Dispose();
        }

        protected Tuple<string, string> GenerateSignatureParameter(ParameterInfo pi)
        {
            if (pi.ParameterType == typeof(String) || pi.ParameterType == typeof(Int32))
            {
                return new Tuple<string, string>(pi.ParameterType.Name, pi.Name);
            }
            else if (typeof(IList).IsAssignableFrom(pi.ParameterType))
            {
                return new Tuple<string, string>("List<" + pi.ParameterType.GenericTypeArguments[0].Name + ">", pi.Name);
            }
            return new Tuple<string, string>("", "");

        }
    }
}
