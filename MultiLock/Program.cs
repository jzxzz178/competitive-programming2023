namespace MultiLock;

class Program
{
    static void Main(string[] args)
    {
        var multiLock = new MultiLock();
        var thread1 = new Thread(() =>
        {
            using var @lock = multiLock.AcquireLock("1", "2", "3");
            Console.WriteLine("1");
            Thread.Sleep(2000);
        });

        var thread2 = new Thread(() =>
        {
            using var @lock = multiLock.AcquireLock("2");
            Console.WriteLine("2");
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
    }
}