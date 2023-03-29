using System.Net;
using System.Net.Sockets;
using System.Text;

const string serverAdress = "127.0.0.1";
const short serverPort = 4040;
const string JOIN_CMD = "$<join>";



HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();

UdpClient server = new UdpClient(serverPort);

IPEndPoint clientEndPoint = null;

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
        default:
            SendToAll(data);
            break;
    }
}

void AddMember(IPEndPoint member)
{
    members.Add(member);

}

void SendToAll(byte[] data)
{
    foreach (var m in members)
    {
        server.SendAsync(data, data.Length, m);
    }
}
