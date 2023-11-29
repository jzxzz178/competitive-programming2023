using CustomThreadPool.ThreadPools;

namespace CustomThreadPool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // var t = new ThreadPools.CustomThreadPool();
            ThreadPoolTests.Run<ThreadPools.CustomThreadPool>();
        }
    }
}