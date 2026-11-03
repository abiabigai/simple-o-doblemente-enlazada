using System.Collections;

namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Generic LIFO stack implemented with dynamic singly linked nodes.
/// The top of the stack is the head of the chain, so Push and Pop are O(1).
/// </summary>
public sealed class DynamicStack<T> : IEnumerable<T>, IDataStructure<T>
{
    private SinglyNode<T>? _top;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public void Push(T item)
    {
        _top = new SinglyNode<T>(item) { Next = _top };
        Count++;
    }

    public T Pop()
    {
        if (_top is null)
            throw new InvalidOperationException("La pila está vacía.");

        T value = _top.Value;
        _top = _top.Next;
        Count--;
        return value;
    }

    public T Peek()
    {
        if (_top is null)
            throw new InvalidOperationException("La pila está vacía.");

        return _top.Value;
    }

    public void Clear()
    {
        _top = null;
        Count = 0;
    }

    /// <summary>Enumerates from the top of the stack to the bottom.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (SinglyNode<T>? node = _top; node is not null; node = node.Next)
            yield return node.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool IDataStructure<T>.TryAdd(T item)
    {
        Push(item);
        return true; // a dynamic stack never overflows (until the process runs out of memory)
    }

    bool IDataStructure<T>.TryRemove(out T? item)
    {
        if (_top is null)
        {
            item = default;
            return false;
        }

        item = Pop();
        return true;
    }
}
