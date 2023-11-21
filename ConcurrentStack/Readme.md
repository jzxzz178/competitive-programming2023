# Задача
- Написать свою реализацию класса lock-free ConcurrentStack. Для этого пригодятся методы класса Interlocked.


- Стек должен реализовывать интерфейс
    ```csharp
    public interface IStack<T>
    {
        void Push(T item);
        bool TryPop(out T item);
        int Count { get; }
    }
    ```
  Свойство Count должно работать за O(1)