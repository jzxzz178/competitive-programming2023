namespace ConcurrentStack;

public class Stack<T> : IStack<T>
{
    private Node<T>? head;

    public Stack()
    {
    }

    public void Push(T item)
    {
        Node<T>? node = new Node<T>(item);
        do
        {
            node.Next = head;
        } while (Interlocked.Exchange<Node<T>?>(ref head, node/*, node.Next*/) != node.Next);
    }

    public bool TryPop(out T item)
    {
        throw new NotImplementedException();
    }

    public int Count { get; }
}

internal class Node<TR>
{
    public TR Value { get; internal set; }
    public Node<TR>? Next { get; internal set; }

    public Node(TR value, Node<TR>? next = null)
    {
        Value = value;
        Next = next;
    }
}