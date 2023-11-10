namespace MultiLock;

public interface IMultiLock
{
    public IDisposable AcquireLock(params string[] keys);
}