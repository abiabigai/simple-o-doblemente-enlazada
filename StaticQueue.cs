using System.Collections;

namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Generic FIFO queue backed by a fixed-size circular array (static memory model).
/// </summary>
public sealed class StaticQueue<T> : IEnumerable<T>, IDataStructure<T>
{
    private readonly T[] _items;
    private int _head;

    public StaticQueue(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _items = new T[capacity];
    }

    public int Capacity => _items.Length;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public bool IsFull => Count == _items.Length;

    public void Enqueue(T item)
    {
        if (IsFull)
            throw new InvalidOperationException($"Desbordamiento de cola: se alcanzó la capacidad de {Capacity}.");

        _items[(_head + Count) % _items.Length] = item;
        Count++;
    }

    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("La cola está vacía.");

        T value = _items[_head];
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        Count--;
        return value;
    }

    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("La cola está vacía.");

        return _items[_head];
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return _items[(_head + i) % _items.Length];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool TryAdd(T item)
    {
        if (IsFull)
            return false;

        Enqueue(item);
        return true;
    }

    public bool TryRemove(out T? item)
    {
        if (IsEmpty)
        {
            item = default;
            return false;
        }

        item = Dequeue();
        return true;
    }
}
