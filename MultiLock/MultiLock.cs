namespace MultiLock;

public class MultiLock : IMultiLock, IDisposable
{
    private readonly object _lockObject = new(); // необходим для потокобезопасного взаимодействия с _multiLockItems
    private readonly Dictionary<string, object> _multiLockItems = new();

    public IDisposable AcquireLock(params string[] keys)
    {
        var allKeysAreAllowed = false;
        try
        {
            foreach (var key in keys)
                EnterKey(key);

            allKeysAreAllowed = true;
            return this;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (!allKeysAreAllowed)
            {
                var enteredKeys = keys.Where(key => Monitor.IsEntered(_multiLockItems[key]));
                foreach (var key in enteredKeys)
                    Monitor.Exit(key);
            }
        }
    }

    private void EnterKey(string key)
    {
        lock (_lockObject)
        {
            if (!_multiLockItems.ContainsKey(key))
                _multiLockItems.Add(key, new object());
            Monitor.Enter(_multiLockItems[key]);
        }
    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            foreach (var item in _multiLockItems
                         .Where(item => Monitor.IsEntered(item.Value)))
                Monitor.Exit(item.Value);
        }
    }
}