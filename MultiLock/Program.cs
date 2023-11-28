namespace MultiLock;

class Program
{
    public static void Main()
    {
        var multiLock = new MultiLock();
        var thread2 = new Thread(() => Work2(multiLock));
        var thread3 = new Thread(() => Work3(multiLock));
        
        thread2.Start();
        thread3.Start();
        
        Console.WriteLine("Блокируем 1 2 3");
        using (multiLock.AcquireLock("1", "2", "3"))
        {
            Console.WriteLine("1 2 3 Заблокированы");
            // Thread.Sleep(500);
            Console.WriteLine("Освобождаем 1 2 3");
        }

        Console.WriteLine("Блокируем 1 2 3");
        using (multiLock.AcquireLock("1", "2", "3"))
        {
            Console.WriteLine("1 2 3 Заблокированы");
        }

        Console.WriteLine("1 2 3 Освобождены");


        thread2.Join();
        thread3.Join();
    }

    private static void Work2(MultiLock multiLock)
    {
        Console.WriteLine("Блокируем 2 3 4 вторым потоком");
        using (multiLock.AcquireLock("2", "3", "4"))
        {
            Console.WriteLine("2 3 4 заблокированы");
            Thread.Sleep(500);
            Console.WriteLine("Освобождаем 2 3 4");
        }
    }

    private static void Work3(MultiLock multiLock)
    {
        Thread.Sleep(100);
        Console.WriteLine("Блокируем 9 третим потоком");
        using (multiLock.AcquireLock("9"))
        {
            Console.WriteLine("9 заблокированы");
            Thread.Sleep(500);
            Console.WriteLine("Освобождаем 9");
        }
    }
}