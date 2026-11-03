namespace DynamicStructuresDemo.Structures;

/// <summary>
/// Dynamic node with a single pointer. Used by the dynamic stack and queue.
/// </summary>
internal sealed class SinglyNode<T>(T value)
{
    public T Value { get; set; } = value;
    public SinglyNode<T>? Next { get; set; }
}
