using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


ChatServer server = new ChatServer();

server.Start();

public class ChatServer
{
    private const string ADDRESS = "127.0.0.1";
    private const short PORT = 4040;
    private const string JOIN_CMD = @"$<join>";
    private const string LEAVE_CMD = "$<leave>";
    private HashSet<(IPEndPoint, string)> members = new HashSet<(IPEndPoint, string)>();
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

            if (msg.Contains(JOIN_CMD))
            {
                if (!_isMaxCount)
                    AddMember(clientEndPoint, msg);
                else
                {
                    byte[] refusal = Encoding.UTF8.GetBytes("Chat already have max count");
                    server.SendAsync(refusal, refusal.Length, clientEndPoint);
                }
            }
            else if (msg == LEAVE_CMD)
            {
                Remove(clientEndPoint);
            }
            else
                SendToAll(data);
        }
    }

    private void AddMember(IPEndPoint member, string msg)
    {
        SendToAll(GetIPAndLogin(msg));
        SendToOne(member);
        members.Add((member, msg));



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
        string login = new string(msg.Except(JOIN_CMD).ToArray());

        return Encoding.UTF8.GetBytes("<ADD.MEMBER>" + "$" + login);
    }

    private void SendToOne(IPEndPoint ip)
    {
        foreach (var m in members)
        {
            byte[] data = GetIPAndLogin(m.Item2);

            server.SendAsync(data, data.Length, ip);
        }
    }

}
