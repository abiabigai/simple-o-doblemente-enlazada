using System.Collections;

namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Generic LIFO stack backed by a fixed-size array (static memory model).
/// The whole capacity is reserved up front, whether it is used or not.
/// </summary>
public sealed class StaticStack<T> : IEnumerable<T>, IDataStructure<T>
{
    private readonly T[] _items;

    public StaticStack(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _items = new T[capacity];
    }

    public int Capacity => _items.Length;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public bool IsFull => Count == _items.Length;

    public void Push(T item)
    {
        if (IsFull)
            throw new InvalidOperationException($"Desbordamiento de pila: se alcanzó la capacidad de {Capacity}.");

        _items[Count++] = item;
    }

    public T Pop()
    {
        if (IsEmpty)
            throw new InvalidOperationException("La pila está vacía.");

        T value = _items[--Count];
        _items[Count] = default!; // release the reference for the garbage collector
        return value;
    }

    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("La pila está vacía.");

        return _items[Count - 1];
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = Count - 1; i >= 0; i--)
            yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool TryAdd(T item)
    {
        if (IsFull)
            return false;

        _items[Count++] = item;
        return true;
    }

    public bool TryRemove(out T? item)
    {
        if (IsEmpty)
        {
            item = default;
            return false;
        }

        item = Pop();
        return true;
    }
}
