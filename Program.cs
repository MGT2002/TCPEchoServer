using MyExtensions;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int BufSize = 32;
const int BackLog = 5;
const int DefaultEchoPort = 7;

if (args.Length > 1)
    throw new ArgumentException("Parameters: [<Port>]");

int servPort = args.Length == 1 ? int.Parse(args[0]) : DefaultEchoPort;

Socket server = null!;
try
{
    server = new(AddressFamily.InterNetwork, SocketType.Stream,
        ProtocolType.Tcp);
    server.Bind(new IPEndPoint(IPAddress.Any, servPort));
    server.Listen(BackLog);
}
catch (SocketException se)
{
    se.ErrorCode.Log(":", se.Message);
    Environment.Exit(se.ErrorCode);
}

byte[] rcvBuffer = new byte[BufSize];
int bytesRcvd;

while (true)
{
    "Accepting Client, Listening to port".Log($"{servPort}...");
    ServeConnection();
}

void ServeConnection()
{
    try
    {
        using Socket client = server.Accept();

        Console.WriteLine("Handling client({0}) at {1} - ", client.LocalEndPoint,
            client.RemoteEndPoint);

        int totalBytesEchoed = 0;
        while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length,
            SocketFlags.None)) > 0)
        {
            Console.WriteLine("Data received : " +
                Encoding.ASCII.GetString(rcvBuffer, 0, bytesRcvd));
            client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
            totalBytesEchoed += bytesRcvd;
        }

        Console.WriteLine($"Echoed {totalBytesEchoed} bytes.");
        Console.WriteLine("Connection closed!");
    }
    catch (Exception e)
    {
        e.Message.Log();
    }
}