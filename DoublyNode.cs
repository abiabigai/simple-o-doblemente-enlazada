namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Dynamic node with two pointers (previous and next). Used by the doubly linked list.
/// </summary>
internal sealed class DoublyNode<T>(T value)
{
    public T Value { get; set; } = value;
    public DoublyNode<T>? Next { get; set; }
    public DoublyNode<T>? Prev { get; set; }
}
