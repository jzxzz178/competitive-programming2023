using Example.MultiLock;

namespace Examples;

public class Program
{
    private static BankAccount vas = new BankAccount("Василий");
    private static BankAccount alex = new BankAccount("Алексей");

    static void Main(string[] args)
    {
        var t1 = new Thread(() =>
        {
            try
            {
                Monitor.Enter(vas);
                // Monitor.Exit(vas);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
        var t2 = new Thread(() =>
        {
            try
            {
                var flag = false;
                Monitor.Enter(vas, ref flag);
                // throw new Exception($"AAAAAAAA {flag}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
        t1.Start();
        t2.Start();
    }

    private static void M()
    {
        try
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}