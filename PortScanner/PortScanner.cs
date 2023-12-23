using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PortScanner;

public class PortScanner : IPScanner
{
    public Task Scan(IPAddress[] ipAddresses, int[] ports)
    {
        var tasksToPing = ipAddresses.Select(ip => StartHandling(ip, ports)).ToList();
        while (tasksToPing.Any(t => !t.IsCompleted))
        {
            Thread.Sleep(1000);    
        }
        return Task.CompletedTask;
    }

    private static Task<PingReply> StartHandling(IPAddress ipAddr, int[] ports, int timeout = 3000)
    {
        using var ping = new Ping();
        Console.WriteLine($"Pinging {ipAddr}");
        var pingTaskAsync = ping.SendPingAsync(ipAddr, timeout);
        
        pingTaskAsync.ContinueWith(task =>
            {
                PingAddr(task.Result, ports);
            },
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent);

        return pingTaskAsync;
    }

    private static Task PingAddr(PingReply pingReply, int[] ports)
    {
        Console.WriteLine($"Pinged {pingReply.Address} with status: {pingReply.Status}");
        
        // if (pingReply.Status != IPStatus.Success)
        //     return Task.FromCanceled(new CancellationToken());

        foreach (var port in ports)
        {
            CheckPort(pingReply, port);
        }

        return Task.CompletedTask;
    }

    private static void CheckPort(PingReply pingReply, int port, int timeout = 3000)
    {
        using var tcpClient = new TcpClient();
        var ipAddr = pingReply.Address;

        Console.WriteLine($"Checking {ipAddr}:{port}");
        var connectAsync = tcpClient.ConnectAsync(ipAddr, port, timeout);
        Console.WriteLine($"Checked {ipAddr}:{port} - {connectAsync.Result}");
    }
}