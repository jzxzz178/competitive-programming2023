using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FirstTask
{
    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    public static class Program
    {
        private static readonly Stopwatch Watch = new Stopwatch();
        private static long _t = 0;
        private static volatile int _id = 0;
        private static readonly BlockingCollection<long> Deltas = new();

        public static void Main(string[] args)
        {
            ProcessAffinity();
            PriorityClass();
            ThreadDemo();
        }

        private static void ProcessAffinity()
        {
            Console.WriteLine(Process.GetCurrentProcess().ProcessorAffinity);
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << Environment.ProcessorCount - 1);
            Console.WriteLine(Process.GetCurrentProcess().ProcessorAffinity);
        }

        private static void PriorityClass()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        }

        private static void ThreadDemo()
        {
            var thread1 = new Thread(Handler)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            var thread2 = new Thread(Handler)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            Watch.Start();
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            var avg = Deltas.Skip(1) // Из-за не одновременного запуска потоков, первая смена контекста происходит долго
                .Average();
            Console.WriteLine("Average: " + avg);
            foreach (var e in Deltas)
            {
                Console.WriteLine("delta: " + e);
            }
        }

        private static void Handler()
        {
            var counter = 0;
            while (counter < 200)
            {
                var currentId = Thread.CurrentThread.ManagedThreadId;
                // Console.WriteLine("id" + currentId);

                if (_id == 0)
                {
                    _id = currentId;
                }

                if (_id != currentId)
                {
                    var newt = Watch.ElapsedMilliseconds;
                    Deltas.Add(newt - _t);
                    _t = newt;
                    _id = currentId;
                    counter++;
                }

                // if (Watch.ElapsedMilliseconds > 10000) break;
            }
        }
    }
}