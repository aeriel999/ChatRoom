using System.Net;
using System.Net.Sockets;
using System.Text;

ChatServer server = new ChatServer();

server.Start();

public class ChatServer
{
    private const string ADDRESS = "127.0.0.1";
    private const short PORT = 4040;
    private const string JOIN_CMD = "$<join>";
    private const string LEAVE_CMD = "$<leave>";
    private HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();
    private UdpClient server = new UdpClient(PORT);
    private IPEndPoint clientEndPoint = null;

    public void Start()
    {
        while (true)
        {
            byte[] data = server.Receive(ref clientEndPoint);

            string msg = Encoding.UTF8.GetString(data);

            Console.WriteLine($"Got : {msg} at {DateTime.Now.ToShortTimeString()} from {clientEndPoint}");

            switch (msg)
            {
                case JOIN_CMD:
                    AddMember(clientEndPoint);
                    break;
                case LEAVE_CMD:
                    Remove(clientEndPoint);
                    break;
                default:
                    SendToAll(data);
                    break;
            }
        }
    }

    private void AddMember(IPEndPoint member)
    {
        members.Add(member);
    }

    private void Remove(IPEndPoint member)
    {
        members.Remove(member);
    }

    private void SendToAll(byte[] data)
    {
        foreach (var m in members)
        {
            server.SendAsync(data, data.Length, m);
        }
    }
}
