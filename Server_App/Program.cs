using CliientApp;
using Command_And_Members;
using Microsoft.IdentityModel.Protocols;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Diagnostics;

using (ChatServer server = new ChatServer())
{
    server.Start();
}

public class ChatServer : IDisposable
{
    private const string ADDRESS = "127.0.0.1";
    private const short PORT = 4040;
    private UdpClient server = new UdpClient(int.Parse(ConfigurationManager.AppSettings["ServerPort"]!));
    private IPEndPoint clientEndPoint = null;
    private const int MAX_OF_MEMBERS = 3;
    private ChatRoomDbContext roomDB = new ChatRoomDbContext(ConfigurationManager.ConnectionStrings["ChatRoomDb"].ConnectionString);

    public void Start()
    {
        while (true)
        {
            try
            {
                byte[] data = server.Receive(ref clientEndPoint);

                string msg = Encoding.UTF8.GetString(data);

                Console.WriteLine($"Got : {msg} at {DateTime.Now.ToShortTimeString()} from {clientEndPoint}");

                if (msg.Contains(Commands.JOIN_CMD))
                    AddMember(clientEndPoint, msg);
                else if (msg.Contains(Commands.LEAVE_CMD))
                    DeleteMember(msg, clientEndPoint);
                else if (msg.Contains(Commands.PRIVATE_CMD))
                    SendInfoForPrivateChate(msg, clientEndPoint);
                else
                    SendMsgToAllMembersMsg(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void SendInfoForPrivateChate(string msg, IPEndPoint sendIp)
    {
        IPEndPoint ip = GetIp(msg);
        byte[] prChat = Encoding.UTF8.GetBytes(Commands.PRIVATE_CMD + GetLogin(sendIp));
        server.SendAsync(prChat, prChat.Length, ip);
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

    private void DeleteMember(string msg, IPEndPoint member)
    {
        Commands.RemoveMemberFromChat(member);

        string login = msg.Substring(Commands.LEAVE_CMD.Length);

        SendMsgToAllMembersMsg(Encoding.UTF8.GetBytes(msg));

        AddOrEditIpInDataBase(login, "Out of net");
    }

    private void AddMember(IPEndPoint member, string msg)
    {
        string login = msg.Substring(Commands.JOIN_CMD.Length);

        if (Commands.IsExist(login))
        {
            byte[] refusal = Encoding.UTF8.GetBytes(Commands.EXIST_CMD);
            server.SendAsync(refusal, refusal.Length, member);
            return;
        }

        if (!IsMaxCountOfMembers())
        {
            SendMsgToAllMembersMsg(Encoding.UTF8.GetBytes(msg));

            SendToNewMemberInfoAboutMembers(member);

            Commands.AddNewMemberToChat(member, login);

            AddOrEditIpInDataBase(login, member.ToString());
        }
        else
        {
            byte[] refusal = Encoding.UTF8.GetBytes("You cant connected. Chat already have max count.");
            server.SendAsync(refusal, refusal.Length, member);
        }
        
        if (IsMaxCountOfMembers())
        {
            SendMsgToAllMembersMsg(Encoding.UTF8.GetBytes("Chat already have max count"));
        }
    }

    private void AddOrEditIpInDataBase(string login, string ipEnd)
    {
        try
        {
            roomDB.Clients.FirstOrDefault(c => c.Login == login).IPEndPoint = ipEnd;
            roomDB.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void SendMsgToAllMembersMsg(byte[] data)
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

    private void SendToNewMemberInfoAboutMembers(IPEndPoint ip)
    {
        foreach (var m in Commands.Members)
        {
            byte[] data = Encoding.UTF8.GetBytes(Commands.JOIN_CMD + m.Item2);

            server.SendAsync(data, data.Length, ip);
        }
    }

    private IPEndPoint GetIp(string msg)
    {
        string login = msg.Substring(Commands.PRIVATE_CMD.Length);

        foreach (var m in Commands.Members)
        {
            if (m.Item2 == login)
                return m.Item1;
        }

        return null;
    }

    public void Dispose()
    {
        roomDB.Dispose();
    }
}
