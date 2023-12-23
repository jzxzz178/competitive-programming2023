using System.Net;

namespace PortScanner
{
    public interface IPScanner
    {
        Task Scan(IPAddress[] ipAddresses, int[] ports);
    }
}