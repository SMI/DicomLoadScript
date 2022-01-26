using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace LoadScript
{
    internal class Config
    {
        public string YearDir { get; set; }
        public string LoadMetadataID { get; set; }

        public string ForLoading { get; set; }

        public string RdmpCli { get; private set; }

        internal static Config MakeUserType()
        {
            Config instance = new Config();

            Console.WriteLine("Enter path to RDMP command line e.g. c:\\rdmp\\rdmp.exe"); ;
            instance.RdmpCli = Console.ReadLine();

            if(!new FileInfo(instance.RdmpCli).Exists)
            {
                Console.WriteLine($"FYI {instance.RdmpCli} does not exist but sure lets carry on");
            }

            Console.WriteLine("Enter Year Directory e.g. G:\\2010\\");
            instance.YearDir = Console.ReadLine();


            Console.WriteLine("Enter ForLoading Directory e.g. C:\\temp\\ImageLoading\\Data\\ForLoading");
            instance.ForLoading = Console.ReadLine();

            Console.WriteLine("Enter RDMP Load ID e.g. 3136");
            instance.LoadMetadataID = Console.ReadLine();

            return instance;
        }
    }
}
