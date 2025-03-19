using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TCP
{
    private TcpListener? _server;
    public event Action<string>? OnDataReceived;
    public event Action<string>? OnStatusUpdated;

    public void StartServer(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
        _server.Start();
        OnStatusUpdated?.Invoke("Server kører...\n");
        _ = AcceptClientsAsync();
    }

    private async Task AcceptClientsAsync()
    {
        while (true)
        {
            var client = await _server!.AcceptTcpClientAsync();
            OnStatusUpdated?.Invoke("Forbundet til Spirometer!\n");
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024*20];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            OnDataReceived?.Invoke(dataReceived);
        }
    }

    public void StopServer()
    {
        _server?.Stop();
        OnStatusUpdated?.Invoke("Server stoppet...\n");
    }
}