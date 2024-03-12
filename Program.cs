using MyExtensions;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int BufSize = 32;
const int DefaultEchoPort = 7;

if (args.Length > 1)
    throw new ArgumentException("Parameters: [<Port>]");

int servPort = args.Length == 1 ? int.Parse(args[0]) : DefaultEchoPort;

TcpListener listener = null!;
try
{
    listener = new(IPAddress.Any, servPort);
    listener.Start();
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
        using TcpClient client = listener.AcceptTcpClient();
        using NetworkStream netStream = client.GetStream();
        Console.WriteLine("Handling client - ");

        while ((bytesRcvd = netStream.Read(rcvBuffer, 0, rcvBuffer.Length)) > 0)
        {
            Console.WriteLine("Data received : " +
                Encoding.ASCII.GetString(rcvBuffer, 0, bytesRcvd));

            netStream.Write(rcvBuffer, 0, bytesRcvd);
            Console.WriteLine($"Echoed {bytesRcvd} bytes.");
        }

        Console.WriteLine("Connection closed!");
    }
    catch (Exception e)
    {
        e.Message.Log();
    }
}