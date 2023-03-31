using System.Net;
using System.Net.Sockets;
using System.Text;

ChatServer server = new ChatServer();

server.Start();



class ChatServer
{
    const string ADDRESS = "127.0.0.1";
    const short PORT = 4040;

    TcpListener listener = null;

    IPEndPoint clientEndPoint = null;

    public ChatServer()
    {
        listener = new TcpListener(IPAddress.Parse(ADDRESS), PORT);
    }

    public void Start()
    {
        listener.Start();

        Console.WriteLine("Waiting for connection .......");

        TcpClient client = listener.AcceptTcpClient();

        Console.WriteLine("Connected!");

        NetworkStream ns = client.GetStream();

        StreamReader reader = new StreamReader(ns);

        StreamWriter writer = new StreamWriter(ns); 

        while (true)
        {
            string msg = reader.ReadLine();

            Console.WriteLine($"Got : {msg} at {DateTime.Now.ToShortTimeString()} from {client.Client.LocalEndPoint}");

            writer.WriteLine("Thanks!");
            writer.Flush();
        }
    }
}



