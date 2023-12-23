using System.Net;
using TPL;

namespace PortScanner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var ipAddresses = new[]
                {IPAddress.Parse("192.168.0.1"), IPAddress.Parse("127.0.0.1") /*, Place your ip addresses here*/};
            var ports = new[] {21, 25, 80, 443, 3389};

            new SequentialScanner().Scan(ipAddresses, ports).Wait();
        }
    }
}