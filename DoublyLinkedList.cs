using System.Collections;

namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Generic doubly linked list built from dynamically allocated nodes.
/// Memory grows and shrinks one node at a time; there is no capacity limit.
/// </summary>
public sealed class DoublyLinkedList<T> : IEnumerable<T>
{
    private DoublyNode<T>? _head;
    private DoublyNode<T>? _tail;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    /// <summary>Gets or sets the element at the given zero-based index. O(n).</summary>
    public T this[int index]
    {
        get
        {
            ValidateIndex(index);
            return GetNode(index).Value;
        }
        set
        {
            ValidateIndex(index);
            GetNode(index).Value = value;
        }
    }

    /// <summary>Inserts a value at the beginning of the list. O(1).</summary>
    public void AddFirst(T value)
    {
        var node = new DoublyNode<T>(value);

        if (_head is null)
        {
            _head = _tail = node;
        }
        else
        {
            node.Next = _head;
            _head.Prev = node;
            _head = node;
        }

        Count++;
    }

    /// <summary>Inserts a value at the end of the list. O(1).</summary>
    public void AddLast(T value)
    {
        var node = new DoublyNode<T>(value);

        if (_tail is null)
        {
            _head = _tail = node;
        }
        else
        {
            node.Prev = _tail;
            _tail.Next = node;
            _tail = node;
        }

        Count++;
    }

    /// <summary>Inserts a value so that it ends up at the given index. O(n).</summary>
    public void InsertAt(int index, T value)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"El índice debe estar entre 0 y {Count}.");

        if (index == 0)
        {
            AddFirst(value);
            return;
        }

        if (index == Count)
        {
            AddLast(value);
            return;
        }

        DoublyNode<T> current = GetNode(index);
        var node = new DoublyNode<T>(value)
        {
            Prev = current.Prev,
            Next = current
        };

        current.Prev!.Next = node;
        current.Prev = node;
        Count++;
    }

    /// <summary>Removes and returns the first element. O(1).</summary>
    public T RemoveFirst()
    {
        if (_head is null)
            throw new InvalidOperationException("La lista está vacía.");

        T value = _head.Value;
        Unlink(_head);
        return value;
    }

    /// <summary>Removes and returns the last element. O(1).</summary>
    public T RemoveLast()
    {
        if (_tail is null)
            throw new InvalidOperationException("La lista está vacía.");

        T value = _tail.Value;
        Unlink(_tail);
        return value;
    }

    /// <summary>Removes and returns the element at the given index. O(n).</summary>
    public T RemoveAt(int index)
    {
        ValidateIndex(index);
        DoublyNode<T> node = GetNode(index);
        T value = node.Value;
        Unlink(node);
        return value;
    }

    /// <summary>Removes the first occurrence of the value. Returns false when not found. O(n).</summary>
    public bool Remove(T value)
    {
        var comparer = EqualityComparer<T>.Default;

        for (DoublyNode<T>? node = _head; node is not null; node = node.Next)
        {
            if (comparer.Equals(node.Value, value))
            {
                Unlink(node);
                return true;
            }
        }

        return false;
    }

    /// <summary>Returns the index of the first occurrence of the value, or -1. O(n).</summary>
    public int IndexOf(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        int index = 0;

        for (DoublyNode<T>? node = _head; node is not null; node = node.Next, index++)
        {
            if (comparer.Equals(node.Value, value))
                return index;
        }

        return -1;
    }

    public bool Contains(T value) => IndexOf(value) >= 0;

    /// <summary>Reverses the list in place by swapping the pointers of every node. O(n).</summary>
    public void Reverse()
    {
        DoublyNode<T>? current = _head;

        while (current is not null)
        {
            (current.Next, current.Prev) = (current.Prev, current.Next);
            current = current.Prev; // after the swap, Prev holds the old Next
        }

        (_head, _tail) = (_tail, _head);
    }

    public void Clear()
    {
        // Dropping the references lets the garbage collector reclaim every node.
        _head = null;
        _tail = null;
        Count = 0;
    }

    /// <summary>Enumerates the list from tail to head.</summary>
    public IEnumerable<T> Backwards()
    {
        for (DoublyNode<T>? node = _tail; node is not null; node = node.Prev)
            yield return node.Value;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (DoublyNode<T>? node = _head; node is not null; node = node.Next)
            yield return node.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"El índice debe estar entre 0 y {Count - 1}.");
    }

    // Walks from the nearest end, so at most Count / 2 nodes are visited.
    private DoublyNode<T> GetNode(int index)
    {
        if (index < Count / 2)
        {
            DoublyNode<T> current = _head!;
            for (int i = 0; i < index; i++)
                current = current.Next!;
            return current;
        }
        else
        {
            DoublyNode<T> current = _tail!;
            for (int i = Count - 1; i > index; i--)
                current = current.Prev!;
            return current;
        }
    }

    private void Unlink(DoublyNode<T> node)
    {
        if (node.Prev is null)
            _head = node.Next;
        else
            node.Prev.Next = node.Next;

        if (node.Next is null)
            _tail = node.Prev;
        else
            node.Next.Prev = node.Prev;

        node.Next = null;
        node.Prev = null;
        Count--;
    }
}
