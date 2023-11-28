namespace ConcurrentStack;

class Program
{
    static void Main()
    {
        var s = new Stack<string>();

        var t1 = new Thread(() =>
        {
            s.Push("2");
            Console.WriteLine($"pushed value: 2; count: {s.Count}");
        });

        var t2 = new Thread(() =>
        {
            s.TryPop(out var value);
            Console.WriteLine($"popped value: {value}; count: {s.Count}");
            
            s.TryPop(out var value2);
            Console.WriteLine($"popped value: {value2}; count: {s.Count}");
        });

        // t1.Start();
        // t2.Start();

        s.Push("1");
        Console.WriteLine($"pushed value: 1; count: {s.Count}");
        s.TryPop(out var item);
        Console.WriteLine($"popped value: {item}; count: {s.Count}");

        // t1.Join();
        // t2.Join();
    }
}