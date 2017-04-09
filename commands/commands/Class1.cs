using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.IO.Compression;
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

        public static string createZipPackageFromDir(string path, string zipname)
        {
            string dirpath = path;
            string zippath = path + "\\" + zipname;
            ZipFile.CreateFromDirectory(dirpath, zippath);
            return zippath;
        }

        public static void extractZipPackage(string zippath, string path)
        {
            ZipFile.ExtractToDirectory(zippath, path);
        }

        public static void createTarGzPackageFromDir(DirectoryInfo dirselect, string path)
        {
            foreach (FileInfo fileToCompress in dirselect.GetFiles())
            {
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) &
                       FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                               CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }
                        FileInfo info = new FileInfo(path + "\\" + fileToCompress.Name + ".gz");
                        Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                        fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                    }

                }
            }

        }

        public static void extractTarGzPackage(FileInfo filesinpackage)
            {
            using (FileStream originalFileStream = filesinpackage.OpenRead())
            {
                string currentFileName = filesinpackage.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - filesinpackage.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", filesinpackage.Name);
                    }
                }
            }
        }
}
        }


namespace connections
{
    public static partial class server
    {
        public static NetworkStream getNetStream(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            return stream;

        }
        public static IPAddress parseip(string ip)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            return localAddr;

        }

        public static TcpListener initServer(IPAddress ip, int port)
        {
            try
            {
                TcpListener servercon = new TcpListener(ip, port);
                return servercon;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }


        }

        public static TcpClient acceptConnection(TcpListener server)
        {
            server.Start();
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connection Successful");
            return client;
        }

        public static string recieveMessage(NetworkStream stream)
        {
            Byte[] bytes = new Byte[256];
            String data = null;
            data = null;
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The Connection was closed");
                data = null;
            }
            return data;
        }
        public static Byte[] processData(string data)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
            return msg;

        }
        public static void returnResponse(Byte[] msg, string data, NetworkStream stream)
        {
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", data);
        }

        public static void closeConnection(TcpClient client)
        {
            client.Close();
        }
    }
    public static partial class client
    {
        public static TcpClient connect(string ip, int port)
        {
            TcpClient con;
            try
            {
                con = new TcpClient(ip, port);
                return con;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static Byte[] sendPacket(string message, NetworkStream stream)
        {
            Byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", data);
            return data;
        }

        public static void getResponse(NetworkStream stream, Byte[] data)
        {
            Byte[] newdata = new Byte[256];
            Int32 bytes = stream.Read(newdata, 0, data.Length);
            String responseData = String.Empty;
            responseData = System.Text.Encoding.ASCII.GetString(newdata, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);
        }

        public static void closeConnections(TcpClient con, NetworkStream stream)
        {
            stream.Close();
            con.Close();
        }
    }


}
namespace nesockets
{
    public static class endpoints
    {
        public static IPHostEntry getHostEntry()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            return ipHost;
        }

        public static IPAddress getAddressList(IPHostEntry ipHost)
        {
            IPAddress ipAddr = ipHost.AddressList[0];
            return ipAddr;
        }

        public static IPEndPoint initEndpoint(IPAddress ipAddr)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
            return ipEndPoint;
        }
    }

    public static partial class serversockets
    {


        public static Socket initSocket()
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return client;
        }




    }
    public static partial class clientsockets
    {
        public static void connectClient(Socket client, IPAddress ipAddr, int port)
        {
            client.Connect(ipAddr, port);
        }

        public static void closeSockets(Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}

