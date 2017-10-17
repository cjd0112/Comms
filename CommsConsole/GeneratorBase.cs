using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
