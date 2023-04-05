using Command_And_Members;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MemberInfo = Command_And_Members.MemberInfo;

ChatServer server = new ChatServer();

server.Start();

public class ChatServer
{
    private const string ADDRESS = "127.0.0.1";
    private const short PORT = 4040;
    private UdpClient server = new UdpClient(PORT);
    private IPEndPoint clientEndPoint = null;
    private const int MAX_OF_MEMBERS = 3;
    private bool _isMaxCount = false;
    private MemberInfo _room = new MemberInfo();
    public ChatServer()
    {
       // _room = 
    }

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
                _room.RemoveMemberFromChat(clientEndPoint);
            }
            else if (msg.Contains(Commands.PRIVATE_CMD))
            {
                IPEndPoint ip = GetIp(msg);
                byte[] prChat = Encoding.UTF8.GetBytes(Commands.NEW_MSG_CMD + GetLogin(clientEndPoint));
                server.SendAsync(prChat, prChat.Length, ip);

                byte[] start = Encoding.UTF8.GetBytes(Commands.OPEN_SENT_CHAT_CMD + ip.ToString());
                server.SendAsync(start, start.Length, clientEndPoint);
            }
            else
                SendToAll(data);
        }
    }

    private string GetLogin(IPEndPoint member)
    {
        foreach (var m in _room.Members)
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
        _room.AddNewMemberToChat(member, login);

        if (IsMaxCountOfMembers())
        {
            SendToAll(Encoding.UTF8.GetBytes("Chat already have max count"));
            _isMaxCount = true;
        }
    }

    private void SendToAll(byte[] data)
    {
        foreach (var m in _room.Members)
        {
            server.SendAsync(data, data.Length, m.Item1);
        }
    }

    private bool IsMaxCountOfMembers()
    {
        if (_room.Members.Count == MAX_OF_MEMBERS)
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
        foreach (var m in _room.Members)
        {
            byte[] data = GetIPAndLogin(m.Item2);

            server.SendAsync(data, data.Length, ip);
        }
    }

    private IPEndPoint GetIp(string msg)
    {
        string login = new string(msg.Except(Commands.PRIVATE_CMD).ToArray());

        foreach (var m in _room.Members)
        { 
            if(m.Item2 == login)
                return m.Item1;
        }

        return null;
    }

}
