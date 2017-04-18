using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.IO.Compression;
using System.Windows.Forms;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace commands
{

        public static partial class clientcom
        {
 public static void addPackage(string path, string zipname)
        {
            
            string zippath = String.Empty;
            try
            {
                zippath = packages.packages.createZipPackageFromDir(path, zipname);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Occured" + ex.Message);
            }

        }

        public static void openPackage(string zippath, string extractpath)
        {
            try
            {
                packages.packages.extractZipPackage(zippath, extractpath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Occured" + ex.Message);
            }
        }
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
            string zippath = Application.StartupPath + zipname;
            ZipFile.CreateFromDirectory(dirpath, zippath,CompressionLevel.Optimal,true);
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
        public static NetworkStream GetNetStream(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            return stream;

        }
        public static IPAddress ParseIp(string ip)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            return localAddr;

        }

        public static TcpListener InitServer(IPAddress ip, int port)
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

        public static TcpClient AcceptConnection(TcpListener server)
        {
            server.Start();
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connection Successful");
            return client;
        }

        public static string RecieveMessage(NetworkStream stream)
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
                Console.WriteLine(ex.Message);
                data = null;
            }
            return data;
        }
        public static Byte[] ProcessData(string data)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
            return msg;

        }
        public static void ReturnResponse(Byte[] msg, string data, NetworkStream stream)
        {
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", data);
        }

        public static void CloseConnection(TcpClient client)
        {
            client.Close();
        }
    }
    public static partial class client
    {
        public static TcpClient Connect(string ip, int port)
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

        public static Byte[] SendPacket(string message, NetworkStream stream)
        {
            Byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", data);
            return data;
        }

        public static void GetResponse(NetworkStream stream, Byte[] data)
        {
            Byte[] newdata = new Byte[256];
            Int32 bytes = stream.Read(newdata, 0, data.Length);
            String responseData = String.Empty;
            responseData = System.Text.Encoding.ASCII.GetString(newdata, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);
        }

        public static void CloseConnections(TcpClient con, NetworkStream stream)
        {
            stream.Close();
            con.Close();
        }
       
    }
public class SSL
    {
        public static X509Certificate CertFromFile(string certfile)
        {
            X509Certificate serverCertificate = X509Certificate.CreateFromCertFile(certfile);
            return serverCertificate;
        }

        static void ProcessClient(TcpClient client, X509Certificate serverCertificate)
        {
            SslStream sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
            
            }

            catch (AuthenticationException aex)
            {


            }
        }

        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                // Read the client's test message.
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();

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

