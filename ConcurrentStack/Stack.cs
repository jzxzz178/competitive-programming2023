namespace ConcurrentStack;

public class Stack<T> : IStack<T>
{
    private Node<T>? head;

    public int Count => count;

    private int count;

    public void Push(T item)
    {
        var spinWait = new SpinWait();
        var node = new Node<T>(value: item, count: head == null ? 0 : head.Count + 1);
        do
        {
            spinWait.SpinOnce();
            node.Next = head;
        } while (Interlocked.CompareExchange(ref head, node, node.Next) != node.Next);

        Interlocked.Increment(ref count);
    }

    public bool TryPop(out T item)
    {
        var spinWait = new SpinWait();
        Node<T>? topNode;
        do
        {
            spinWait.SpinOnce();
            topNode = head;
            if (topNode == null)
            {
                item = default(T);
                return false;
            }
        } while (Interlocked.CompareExchange(ref head, head.Next, topNode) != topNode);

        Interlocked.Decrement(ref count);

        item = topNode.Value;
        return true;
    }
}

internal class Node<TR>
{
    public TR Value { get; internal set; }
    public Node<TR>? Next { get; internal set; }
    public int Count { get; }

    public Node(TR value = default(TR), Node<TR>? next = null, int count = 0)
    {
        Value = value;
        Next = next;
        Count = count;
    }
}