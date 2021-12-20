
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using View_PBL4.Models;
using View_PBL4.BLL;

    namespace View_PBL4.Server_Socket
    {
        public class Server_Socket
        {


            static byte[] clientData = new byte[1024];
            static IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 9000);
            static Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            static readonly List<Socket> list_clients = new List<Socket>();
            static int count = 1;
            static bool check = false;
            static bool checktext = false;
            public static bool CheckNameFile = true;
            public static void Run()
            {
                // ServerSocket.Start();
                ServerSocket.Bind(ipEnd);
                ServerSocket.Listen(9000);
                System.Diagnostics.Debug.WriteLine(" >>  Server Started");
                /*new Thread(delegate ()
                {
                    while (true)
                    {
                        Socket client = ServerSocket.Accept();
                        list_clients.Add(client);

                    }
                }).Start();*/
                while (true)
                {
                Socket client = ServerSocket.Accept();
                list_clients.Add(client);

                foreach (Socket client1 in list_clients)
                    {
                        User user = new User();
                        user.UserID = GetClientMAC(client1.RemoteEndPoint.ToString());
                        user.Detail = Convert.ToString(client1.RemoteEndPoint);
                        user.Status = "Online";
                        if (BLL_Controller.Instance.CheckUserID(user.UserID))
                        {
                            BLL_Controller.Instance.AddUserSocket(user);
                        }
                        else
                        {
                        BLL_Controller.Instance.UpdateStatus(GetClientMAC(client.RemoteEndPoint.ToString()),
                                                     Convert.ToString(client.RemoteEndPoint),
                                                     "Online");
                    }
                    }
                }
            }
            public static void AcceptRequestFromWeb(string MacAddress,string Request)
            {
            foreach (Socket client in list_clients)
            {
                 if (MacAddress == GetClientMAC(client.RemoteEndPoint.ToString()))
                //if (MacAddress.Trim() == client.RemoteEndPoint.ToString())
                 {
                    Console.WriteLine("Request to Client {1} with IP {0} is ", client.RemoteEndPoint.ToString(), GetClientMAC(client.RemoteEndPoint.ToString()));

                    Runsever(client,Request);

                 }
            }
            }
            
            public static void ShowSever(object o)
            {
                int id = (int)o;
                if (list_clients.Count != 0)
                {
                    /*Socket client;
                    lock (_lock) client = list_clients[list_clients.Count - 1];*/
                    foreach (Socket c in list_clients)
                    {
                        Console.WriteLine("Client {1} connected  with IP {0}", c.RemoteEndPoint.ToString(), GetClientMAC(c.RemoteEndPoint.ToString()));
                    }
                }

            }
            public static void SendRequestToApp(string request, Socket client)
            {
                byte[] commandtextData = Encoding.ASCII.GetBytes(request);
                //client.Send(commandtext);


                byte[] commandtext = new byte[4 + commandtextData.Length];
                byte[] commandtextDataLength = BitConverter.GetBytes(commandtextData.Length);
                commandtextDataLength.CopyTo(commandtext, 0);
                commandtextData.CopyTo(commandtext, 4);


                client.Send(commandtext);
                Console.WriteLine("----------request----------------");
                //ShowSever(count);
                Array.Clear(commandtext, 0, commandtext.Length);
            }
            public static void Runsever(Socket client,string request)
            {
                /*Console.WriteLine("Nhap request");
                string request = Console.ReadLine();*/

                Console.WriteLine("Choose the request to client is {0}", request);
                switch (request.ToLower().Trim())
                {
                    case "shutdown":
                        //SendRequestToApp(request, client);

                        new Thread(delegate ()
                        {
                            handle_clients(client, "shutdown");
                        }
                        )
                        .Start();
                    //handle_clients(count);
                    BLL_Controller.Instance.UpdateStatus(GetClientMAC(client.RemoteEndPoint.ToString()),
                                                         Convert.ToString(client.RemoteEndPoint), 
                                                         "Offline");
                    client.Close();
                    ; break;
                    case "restart":
                        //SendRequestToApp(request, client);

                        new Thread(delegate ()
                        {
                            handle_clients(client, "restart");
                        }
                        )
                        .Start();
                        //handle_clients(count);
                        BLL_Controller.Instance.UpdateStatus(GetClientMAC(client.RemoteEndPoint.ToString()),
                                                         Convert.ToString(client.RemoteEndPoint), 
                                                         "Offline");
                    client.Close();
                    ; break;
                    case "sendtext":

                        Console.WriteLine("Starting TCP listener");
                        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 8000);
                        Socket socktext = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socktext.Bind(ipEnd);
                        int countertext = 0;
                        socktext.Listen(9000);


                        checktext = false;
                        Console.WriteLine(client.RemoteEndPoint.ToString());
                        SendRequestToApp(request, client);
                        while (true)
                        {
                            try
                            {
                                Socket clientSock = socktext.Accept();
                                IPEndPoint remoteIpEndPoint = clientSock.RemoteEndPoint as IPEndPoint;
                                IPEndPoint localIpEndPoint = clientSock.LocalEndPoint as IPEndPoint;
                                Console.WriteLine(">> Client No:" + Convert.ToString(countertext) + "started!!!");

                                //Console.WriteLine(clientSock.RemoteEndPoint.ToString());
                                if (remoteIpEndPoint != null)
                                {
                                    Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + "on port number " + remoteIpEndPoint.Port);
                                }
                                if (localIpEndPoint != null)
                                {
                                    Console.WriteLine("My local IpAddress is :" + localIpEndPoint.Address + "I am connected on port number " + localIpEndPoint.Port);
                                }
                                Console.WriteLine(">> Client No:" + Convert.ToString(countertext) + "started!!!");
                                int totalBytesRead = 0;
                                int bytesRead = 10;
                                byte[] clientData = new byte[1024 * 10000];

                                while ((bytesRead = clientSock.Receive(clientData, totalBytesRead,
                                clientData.Length - totalBytesRead, SocketFlags.None)) != 0)
                                {
                                    totalBytesRead += bytesRead;
                                }

                                doText(clientData, totalBytesRead, client);
                                Array.Clear(clientData, 0, clientData.Length);
                                totalBytesRead = 0;

                                countertext += 1;
                                //clientSock.Close();
                                if (checktext)
                                {
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                        }
                        socktext.Close();
                        //count++;
                        ; break;
                    case "sendimg":

                        Console.WriteLine("Starting TCP listener");
                        IPEndPoint ipEnd2 = new IPEndPoint(IPAddress.Any, 8000);
                        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        sock.Bind(ipEnd2);
                        int counter = 0;
                        sock.Listen(9000);

                        check = false;
                        SendRequestToApp(request, client);
                        while (true)
                        {
                            try
                            {
                                //List<string> listFolder = BLL_ControllerInstance.GetIDFolder();

                                Socket clientSock = sock.Accept();
                                IPEndPoint remoteIpEndPoint = clientSock.RemoteEndPoint as IPEndPoint;
                                IPEndPoint localIpEndPoint = clientSock.LocalEndPoint as IPEndPoint;
                                Console.WriteLine(">> Client No:" + Convert.ToString(counter) + "started!!!");
                                if (remoteIpEndPoint != null)
                                {
                                    Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + "on port number " + remoteIpEndPoint.Port);
                                }

                                if (localIpEndPoint != null)
                                {
                                    Console.WriteLine("My local IpAddress is :" + localIpEndPoint.Address + "I am connected on port number " + localIpEndPoint.Port);
                                }
                                int totalBytesRead = 0;
                                int bytesRead = 10;
                                byte[] clientData = new byte[1024 * 10000];

                                while ((bytesRead = clientSock.Receive(clientData, totalBytesRead,
                                clientData.Length - totalBytesRead, SocketFlags.None)) != 0)
                                {
                                    totalBytesRead += bytesRead;
                                }

                                doChat(clientData, totalBytesRead, client);
                                Array.Clear(clientData, 0, clientData.Length);
                                totalBytesRead = 0;

                                counter += 1;
                                clientSock.Close();
                                if (check)
                                {
                                    break;
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        sock.Close();
                        ; break;

                    default:; break;
                }
            }
            public static void handle_clients(Socket client, string data)
            {
                /*int id = (int)o;
                Socket client;
                lock (_lock) client = list_clients[id-1];*/
                /*while (true)
                {*/

                /*Console.Write("Client address: ");
                string Macaddress = Console.ReadLine();*/
                //broadcast(Macaddress);

                /* foreach (Socket c in list_clients.Values)
                 {
                     // if (Macaddress == GetClientMAC(c.RemoteEndPoint.ToString()))
                     if (Macaddress.Trim() == c.RemoteEndPoint.ToString())
                     {*/
                Console.WriteLine("Your request to Client {1} with IP {0} is {2}", client.RemoteEndPoint.ToString(), GetClientMAC(client.RemoteEndPoint.ToString()), data);


                SendRequestToApp(data, client);


                // }
                //}
                //}
                /*lock (_lock) list_clients.Remove(id);
                client.Shutdown(SocketShutdown.Both);
                client.Close();*/
            }
            public static void broadcast(string Macaddress)
            {
                {
                    foreach (Socket c in list_clients)
                    {
                        // if (Macaddress == GetClientMAC(c.RemoteEndPoint.ToString()))
                        if (Macaddress.Trim() == c.RemoteEndPoint.ToString())
                        {
                            Console.WriteLine("Your request to Client {1} with IP {0}", c.RemoteEndPoint.ToString(), GetClientMAC(c.RemoteEndPoint.ToString()));
                            string data = Console.ReadLine();

                            SendRequestToApp(data, c);
                            //Array.Clear(command, 0, command.Length);
                        }
                    }
                }
            }
            public static void doText(byte[] clientData, int receivedBytesLen, Socket clientSocket)
            {
                string filePath = @"D:\ITstd\PBL4\TCP_Socket\Server\save\Text\";//Your File Path;
                Console.WriteLine("getting file...");

                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                if (fileNameLen <= 4)
                {
                    checktext = true;
                }
                else
                {
                    int folderNameLen = BitConverter.ToInt32(clientData, 4);
                    //int folderNameLen = BitConverter.ToInt32(clientData, 4);
                    string fileName = Encoding.ASCII.GetString(clientData, 8, fileNameLen);
                    string folderName = Encoding.ASCII.GetString(clientData, 8 + fileNameLen, folderNameLen);


                    Folder foldertext = new Folder();
                    ///List<string> listFolder = BLL.BLL_SocketClient.Instance.GetIDFolder();
                    foldertext.FolderID = folderName;
                    foldertext.Time = DateTime.Today.ToString();
                    foldertext.Type = "Text";
                    foldertext.UserID = GetClientMAC(clientSocket.RemoteEndPoint.ToString());
                    if (BLL_Controller.Instance.CheckFolderID(folderName))
                    {

                        BLL_Controller.Instance.AddFolder(foldertext);
                    }



                    Console.WriteLine(folderName);
                    Console.WriteLine(fileName);
                    string directoryText = filePath + folderName + "\\";


                    FileDetail fileDetail = new FileDetail();
                    fileDetail.FileID = fileName + " IN " + folderName;
                    fileDetail.FolderID = folderName;
                    fileDetail.Time = DateTime.Now.ToString();
                    //fileDetail.Link = directoryText;
                    fileDetail.Link = fileName;
                    if (BLL_Controller.Instance.CheckFileDetail(fileName + " IN " + folderName))
                    {
                        BLL_Controller.Instance.AddFileDetail(fileDetail);
                    }


                    if (!Directory.Exists(directoryText))
                    {
                        Directory.CreateDirectory(directoryText);
                    }
                    BinaryWriter bWrite = new BinaryWriter(File.Open(directoryText + fileName, FileMode.Append));

                    bWrite.Write(clientData, 8 + folderNameLen + fileNameLen, receivedBytesLen - 8 - fileNameLen - folderNameLen);
                }
            }
            public static void doChat(byte[] clientData, int receivedBytesLen, Socket client)
            {

                string filePath = @"D:\ITstd\PBL4\TCP_Socket\Server\save\Image\";//Your File Path;
                Console.WriteLine("getting file...");
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                if (fileNameLen <= 4)
                {
                    check = true;
                }
                else
                {
                    int folderNameLen = BitConverter.ToInt32(clientData, 4);
                    //int folderNameLen = BitConverter.ToInt32(clientData, 4);
                    string fileName = Encoding.ASCII.GetString(clientData, 8, fileNameLen);
                    string folderName = Encoding.ASCII.GetString(clientData, 8 + fileNameLen, folderNameLen);


                    Folder folder = new Folder();
                    ///List<string> listFolder = BLL.BLL_SocketClient.Instance.GetIDFolder();
                    folder.FolderID = folderName;
                    folder.Time = DateTime.Today.ToString();
                    folder.Type = "Image";
                    folder.UserID = GetClientMAC(client.RemoteEndPoint.ToString());
                    if (BLL_Controller.Instance.CheckFolderID(folderName))
                    {

                        BLL_Controller.Instance.AddFolder(folder);
                    }
                    //ListFolder.Add(folder);


                    if (fileName == "done")
                    {
                        CheckNameFile = false;
                        return;
                    }
                    Console.WriteLine(folderName);
                    Console.WriteLine(fileName);

                    string directoryImage = filePath + folderName + "\\";
                    FileDetail fileDetail = new FileDetail();
                    fileDetail.FileID = fileName;
                    fileDetail.FolderID = folderName;
                    fileDetail.Time = DateTime.Now.ToString();
                    fileDetail.Link = directoryImage;
                    if (BLL_Controller.Instance.CheckFileDetail(fileName))
                    {
                        BLL_Controller.Instance.AddFileDetail(fileDetail);
                    }

                    //ListFile.Add(fileDetail);

                    if (!Directory.Exists(directoryImage))
                    {
                        Directory.CreateDirectory(directoryImage);
                    }
                    if (!Directory.Exists(fileName))
                    {
                        BinaryWriter bWrite = new BinaryWriter(File.Open(directoryImage + fileName, FileMode.Append));

                        bWrite.Write(clientData, 8 + folderNameLen + fileNameLen, receivedBytesLen - 8 - fileNameLen - folderNameLen);
                    }
                }

            }
            [DllImport("Iphlpapi.dll")]
            private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
            [DllImport("Ws2_32.dll")]
            private static extern Int32 inet_addr(string ip);
            private static string GetClientMAC(string strClientIP)
            {
                string mac_dest = "";
                try
                {
                    Int32 ldest = inet_addr(strClientIP);
                    Int32 lhost = inet_addr("");
                    Int64 macinfo = new Int64();
                    Int32 len = 6;
                    int res = SendARP(ldest, 0, ref macinfo, ref len);
                    string mac_src = macinfo.ToString("X");
                    while (mac_src.Length < 12)
                    {
                        mac_src = mac_src.Insert(0, "0");
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        if (0 == (i % 2))
                        {
                            if (i == 10)
                            {
                                mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                            }
                            else
                            {
                                mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    throw new Exception("L?i " + err.Message);
                }
                return mac_dest;
            }
        }
    }
