using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FirstTask
{
    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    public static class Program
    {
        private static readonly Stopwatch Watch = new();
        private static long _previousMeasurement;
        private static volatile int _previousId;
        private static readonly BlockingCollection<long> Intervals = new();

        public static void Main(string[] args)
        {
            ProcessAffinity();
            PriorityClass();
            ThreadDemo();
        }

        private static void ProcessAffinity()
        {
            // Console.WriteLine(Environment.ProcessorCount);
            // Console.WriteLine(Process.GetCurrentProcess().ProcessorAffinity);
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << Environment.ProcessorCount - 1);
            // Console.WriteLine(Process.GetCurrentProcess().ProcessorAffinity);
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

            var avg = Intervals
                .Skip(1) // Из-за не одновременного запуска потоков, первая смена контекста происходит долго
                .Average();
            Console.WriteLine("Average: " + avg);
            // foreach (var e in Intervals)
            // {
            //     Console.WriteLine("delta: " + e);
            // }
        }

        private static void Handler()
        {
            var counter = 0;
            while (counter < 200)
            {
                var currentId = Environment.CurrentManagedThreadId;

                if (_previousId == 0)
                {
                    _previousId = currentId;
                }

                if (_previousId != currentId)
                {
                    var measurement = Watch.ElapsedMilliseconds;
                    Intervals.Add(measurement - _previousMeasurement);
                    _previousMeasurement = measurement;
                    _previousId = currentId;
                    counter++;
                }

                if (Watch.ElapsedMilliseconds > 20000) break;
            }
        }
    }
}