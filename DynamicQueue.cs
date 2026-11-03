using System.Collections;

namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Generic FIFO queue implemented with dynamic singly linked nodes.
/// Keeps a pointer to both ends, so Enqueue and Dequeue are O(1).
/// </summary>
public sealed class DynamicQueue<T> : IEnumerable<T>, IDataStructure<T>
{
    private SinglyNode<T>? _head;
    private SinglyNode<T>? _tail;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public void Enqueue(T item)
    {
        var node = new SinglyNode<T>(item);

        if (_tail is null)
            _head = _tail = node;
        else
        {
            _tail.Next = node;
            _tail = node;
        }

        Count++;
    }

    public T Dequeue()
    {
        if (_head is null)
            throw new InvalidOperationException("La cola está vacía.");

        T value = _head.Value;
        _head = _head.Next;

        if (_head is null)
            _tail = null;

        Count--;
        return value;
    }

    public T Peek()
    {
        if (_head is null)
            throw new InvalidOperationException("La cola está vacía.");

        return _head.Value;
    }

    public void Clear()
    {
        _head = null;
        _tail = null;
        Count = 0;
    }

    /// <summary>Enumerates from the front of the queue to the back.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (SinglyNode<T>? node = _head; node is not null; node = node.Next)
            yield return node.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool IDataStructure<T>.TryAdd(T item)
    {
        Enqueue(item);
        return true;
    }

    bool IDataStructure<T>.TryRemove(out T? item)
    {
        if (_head is null)
        {
            item = default;
            return false;
        }

        item = Dequeue();
        return true;
    }
}
