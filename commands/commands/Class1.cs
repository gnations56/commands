using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace commands
{
        public static partial class servercom
        {


        }

        public static partial class clientcom
        {


        }


   

    
}

namespace packages
{
    public static class packages
    {

        public static string[] loadFile(string path)
        {
            string[] file = File.ReadAllLines(path);

            return file;

        }

        public static string getFileName(string path)
        {
            string filename = Path.GetFileName(path);
            return filename;


        }
        public static string getTimestamp()
        {
            string ts = DateTime.Now.ToString();
            return ts;

        }
        public static List<List<string>> registerPackage(string timestamp, string filename, List<List<string>> registeredPackages)
        {
            List<string> package = new List<string> { timestamp, filename };
            registeredPackages.Add(package);
            Console.WriteLine("Package added successfully");
            return registeredPackages;
        }

        public static List<List<string>> deregisterPackage(string timestamp, string filename, List<List<string>> registeredPackages)
        {
            List<string> package = new List<string> { timestamp, filename };

            registeredPackages.Remove(package);
            Console.WriteLine("Package removed successfully");
            return registeredPackages;
        }

       
            }
        }


namespace connections
{

}