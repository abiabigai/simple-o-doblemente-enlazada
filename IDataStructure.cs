namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Minimal common contract used by the comparison benchmark so that static
/// (array based) and dynamic (node based) structures can be measured the same way.
/// </summary>
public interface IDataStructure<T>
{
    int Count { get; }

    /// <summary>Adds an element (Push / Enqueue). Returns false when there is no room.</summary>
    bool TryAdd(T item);

    /// <summary>Removes an element (Pop / Dequeue). Returns false when the structure is empty.</summary>
    bool TryRemove(out T? item);
}
