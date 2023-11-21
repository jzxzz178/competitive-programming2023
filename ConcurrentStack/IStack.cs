namespace ConcurrentStack;

public interface IStack<T>
{
    void Push(T item);
    bool TryPop(out T item);
    int Count { get; }
}