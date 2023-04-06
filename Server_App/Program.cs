using CliientApp;
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
    private UdpClient server = new UdpClient(PORT);
    private IPEndPoint clientEndPoint = null;
    private const int MAX_OF_MEMBERS = 3;
    private bool _isMaxCount = false;
    private ChatRoomDB roomDB = new ChatRoomDB();

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
                Commands.RemoveMemberFromChat(clientEndPoint);
                string login = new string(msg.Except(Commands.LEAVE_CMD).ToArray());

                roomDB.Clients.FirstOrDefault(c => c.Login == login).IPEndPoint = "Out of net"; 
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
                SendToAllMembersLoginOfNewMember(data);
        }
    }

    private string GetLogin(IPEndPoint member)
    {
        foreach (var m in Commands.Members)
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
        SendToAllMembersLoginOfNewMember(GetILoginForListMembers(msg));
        SendToNewMemberInfoAboutMembers(member);
        string login = new string(msg.Except(Commands.JOIN_CMD).ToArray());
        Commands.AddNewMemberToChat(member, login);
        roomDB.Clients.FirstOrDefault(c => c.Login == login).IPEndPoint = member.ToString();
        roomDB.SaveChanges();

        if (IsMaxCountOfMembers())
        {
            SendToAllMembersLoginOfNewMember(Encoding.UTF8.GetBytes("Chat already have max count"));
            _isMaxCount = true;
        }
    }

    private void SendToAllMembersLoginOfNewMember(byte[] data)
    {
        foreach (var m in Commands.Members)
        {
            server.SendAsync(data, data.Length, m.Item1);
        }
    }

    private bool IsMaxCountOfMembers()
    {
        if (Commands.Members.Count == MAX_OF_MEMBERS)
            return true;
        else
            return false;
    }

    private byte[] GetILoginForListMembers(string msg)
    {
        string login = new string(msg.Except(Commands.JOIN_CMD).ToArray());

        return Encoding.UTF8.GetBytes(Commands.ADD_CMD + login);
    }

    private void SendToNewMemberInfoAboutMembers(IPEndPoint ip)
    {
        foreach (var m in Commands.Members)
        {
            byte[] data = GetILoginForListMembers(m.Item2);

            server.SendAsync(data, data.Length, ip);
        }
    }

    private IPEndPoint GetIp(string msg)
    {
        string login = new string(msg.Except(Commands.PRIVATE_CMD).ToArray());

        foreach (var m in Commands.Members)
        {
            if (m.Item2 == login)
                return m.Item1;
        }

        return null;
    }
}
