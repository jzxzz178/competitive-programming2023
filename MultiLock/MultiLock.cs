namespace MultiLock;

public class MultiLock : IMultiLock, IDisposable
{
    private readonly object _lockObject = new(); // необходим для потокобезопасного взаимодействия с _multiLockItems
    private readonly Dictionary<string, object> _multiLockItems = new();

    public IDisposable AcquireLock(params string[] keys)
    {
        var allKeysAreAllowed = true;
        try
        {
            if (keys.Any(key => !TryToEnterKey(key)))
            {
                allKeysAreAllowed = false;
            }

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

    private bool TryToEnterKey(string key)
    {
        lock (_lockObject)
        {
            if (!_multiLockItems.ContainsKey(key))
                _multiLockItems.Add(key, new object());

            if (Monitor.IsEntered(_multiLockItems[key])) 
                return false;
            
            Monitor.Enter(_multiLockItems[key]);
            return true;
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