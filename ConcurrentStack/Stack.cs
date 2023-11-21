namespace ConcurrentStack;

public class Stack<T> : IStack<T>
{
    private Node<T>? head;

    public Stack()
    {
    }

    public void Push(T item)
    {
        var node = new Node<T>(item);
        do
        {
            node.Next = head;
        } while (Interlocked.CompareExchange(ref head, node, node.Next) != node.Next);

        var count = Count;
        Interlocked.Increment(ref count);
        Count = count;
    }

    public bool TryPop(out T? item)
    {
        Node<T>? topNode;
        do
        {
            topNode = head;
            if (topNode == null)
            {
                item = default(T);
                return false;
            }
        } while (Interlocked.CompareExchange(ref head, head.Next, topNode) != topNode);

        var count = Count;
        Interlocked.Decrement(ref count);
        // if (Interlocked.Increment(ref count) > 0)
        //     Count = count;

        item = topNode.Value;
        return true;
    }

    public int Count { get; private set; }
}

internal class Node<TR>
{
    public TR Value { get; internal set; }
    public Node<TR>? Next { get; internal set; }

    public Node(TR value = default(TR), Node<TR>? next = null)
    {
        Value = value;
        Next = next;
    }
}