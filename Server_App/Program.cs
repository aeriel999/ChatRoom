using Command_And_Members;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;



ChatServer server = new ChatServer();

server.Start();

public class ChatServer
{
    private const string ADDRESS = "127.0.0.1";
    private const short PORT = 4040;
   
    private HashSet<(IPEndPoint, string)> members = new HashSet<(IPEndPoint, string)>();//make identity
    private UdpClient server = new UdpClient(PORT);
    private IPEndPoint clientEndPoint = null;
    private const int MAX_OF_MEMBERS = 3;
    private bool _isMaxCount = false;

    public void Start()
    {
        while (true)
        {
            byte[] data = server.Receive(ref clientEndPoint);

            string msg = Encoding.UTF8.GetString(data);

            Console.WriteLine($"Got : {msg} at {DateTime.Now.ToShortTimeString()} from {clientEndPoint}");

            if (msg.Contains(Commands.JOIN_CMD))
            {
                if (!_isMaxCount)
                    AddMember(clientEndPoint, msg);
                else
                {
                    byte[] refusal = Encoding.UTF8.GetBytes("Chat already have max count");
                    server.SendAsync(refusal, refusal.Length, clientEndPoint);
                }
            }
            else if (msg == Commands.LEAVE_CMD)
            {
                Remove(clientEndPoint);
            }
            else if (msg.Contains(Commands.PRIVATE_CMD))
            {
                IPEndPoint ip = GetIp(msg);
                byte[] prChat = Encoding.UTF8.GetBytes(Commands.NEWMSG_CMD + GetLogin(clientEndPoint));
                server.SendAsync(prChat, prChat.Length, ip);

                byte[] start = Encoding.UTF8.GetBytes(Commands.OPENCHAT_CMD + ip.ToString());
                server.SendAsync(start, start.Length, clientEndPoint);
            }
            else
                SendToAll(data);
        }
    }

    private string GetLogin(IPEndPoint member)
    {
        foreach (var m in members)
        {
            if (m.Item1.ToString() == member.ToString())
            {
                return m.Item2;
            }
        }

        return null;
    }

    private void AddMember(IPEndPoint member, string msg)
    {
        SendToAll(GetIPAndLogin(msg));
        SendToOneAboutMembers(member);
        string login = new string(msg.Except(Commands.JOIN_CMD).ToArray());
        members.Add((member, login));

        if (IsMaxCountOfMembers())
        {
            SendToAll(Encoding.UTF8.GetBytes("Chat already have max count"));
            _isMaxCount = true;
        }
    }

    private void Remove(IPEndPoint member)
    {
        foreach (var m in members)
        {
            if(m.Item1 == member)
                members.Remove(m);
        }
    }

    private void SendToAll(byte[] data)
    {
        foreach (var m in members)
        {
            server.SendAsync(data, data.Length, m.Item1);
        }
    }

    private bool IsMaxCountOfMembers()
    {
        if (members.Count == MAX_OF_MEMBERS)
            return true;
        else
            return false;
    }

    private byte[] GetIPAndLogin(string msg)
    {
        string login = new string(msg.Except(Commands.JOIN_CMD).ToArray());

        return Encoding.UTF8.GetBytes(Commands.ADD_CMD + login);
    }

    private void SendToOneAboutMembers(IPEndPoint ip)
    {
        foreach (var m in members)
        {
            byte[] data = GetIPAndLogin(m.Item2);

            server.SendAsync(data, data.Length, ip);
        }
    }

    private IPEndPoint GetIp(string msg)
    {
        string login = new string(msg.Except(Commands.PRIVATE_CMD).ToArray());

        foreach (var m in members)
        { 
            if(m.Item2 == login)
                return m.Item1;
        }

        return null;
    }

    //private void TctConnect(IPEndPoint ip)
    //{
    //    IPAddress iPAddress = IPAddress.Parse(ADDRESS);
    //    IPEndPoint ipPoint = new IPEndPoint(iPAddress, PORT);
    //    TcpListener listener = new TcpListener(ipPoint);
    //    StreamReader sr = null;
    //    NetworkStream ns = null;

    //    try
    //    {
    //        listener.Start(10);

    //        Console.WriteLine("Server started! Waiting for connection...");

    //        TcpClient server = listener.AcceptTcpClient();

    //        Console.WriteLine("Connected!");

    //        while (server.Connected)
    //        {
    //            ns = server.GetStream();

    //            sr = new StreamReader(ns);
    //            string response = sr.ReadLine();

    //            Console.WriteLine($"{server.Client.RemoteEndPoint} - {response} at {DateTime.Now.ToShortTimeString()}");
    //        }
    //        server.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //    finally
    //    {
    //        sr.Close();
    //        ns.Close();
    //        listener.Stop();
    //    }
    //}

    //private void SentPrivateMsg(string msg, IPEndPoint ip)
    //{
    //    StreamWriter sw = null;
    //    NetworkStream ns = null;


    //    while (true)
    //    {
    //        TcpClient  client = new TcpClient();

    //        client.Connect(ip);

    //        ns = client.GetStream();

    //        sw = new StreamWriter(ns);  
    //        sw.WriteLine(msg);

    //        sw.Flush();
    //    }
    //}
}
