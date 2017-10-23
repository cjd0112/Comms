using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AmlClient.AS.Application;
using Comms;
using CsvHelper;
using Logger;
using MajordomoProtocol;
using NetMQ;
using Newtonsoft.Json;
using Shared;
using StructureMap;

namespace AmlClient
{
    public class Retail
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String CompanyName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Town { get; set; }
        public String Country { get; set; }
        public String PostCode { get; set; }
        public String Telephone1 { get; set; }
        public String Telephone2 { get; set; }
        public String Email { get; set; }
        public String WebAddress { get; set; }
        public String TxnProfile { get; set; }

    }
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                Container c = null;
                var reg = new MyRegistry(args.Any() == false ? "appsettings.json" : args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);

                var clientFactory = new ClientFactory(c);
                

                var z = clientFactory.GetClient<IFuzzyMatcher>(0);

                CsvReader rdr = new CsvReader(new StreamReader(@"C:\home\colin\as\input\Retail-Small.csv"));
                var records = rdr.GetRecords<Retail>();
                List<FuzzyWordEntry> fwe = new List<FuzzyWordEntry>();
                records.Do(x=>fwe.Add(new FuzzyWordEntry{DocId=Int32.Parse(x.Id),Phrase=x.Name}));
                z.AddEntry(fwe);
                Console.ReadLine();
                L.CloseLog();


            }
            catch (Exception e)
            {
                L.Exception(e);
            }
        }

      
    }
}
