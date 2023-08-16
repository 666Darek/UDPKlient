using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class UdpCommunicator
{
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private CancellationTokenSource cts = new CancellationTokenSource();

    public UdpCommunicator(int localPort)
    {
        udpClient = new UdpClient(localPort);
    }

    public void Send(string message, string remoteIpAddress, int remotePort)
    {
        byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpAddress), remotePort);
        udpClient.Send(bytesToSend, bytesToSend.Length, remoteEndPoint);
    }

    public string Receive()
    {
        endPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedBytes = udpClient.Receive(ref endPoint);
        return Encoding.UTF8.GetString(receivedBytes);
    }

    public async Task<string> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            UdpReceiveResult result = await udpClient.ReceiveAsync();
            return Encoding.UTF8.GetString(result.Buffer);
        }

        return null;  // Zwróć null lub odpowiednią wiadomość w przypadku anulowania
    }

    public void TaskReceive()
    {
        Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                string receivedMessage = await ReceiveAsync(cts.Token);
                if (receivedMessage != null)
                {
                    Console.WriteLine($"Otrzymano: {receivedMessage}");
                }
            }
        });
    }

    public void StopListening()
    {
        cts.Cancel();
    }

    public void Close()
    {
        StopListening();
        udpClient.Close();
    }
}
