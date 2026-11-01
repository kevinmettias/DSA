using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.DataStructures.Deque;

// CircularBuffer with no bound (ARCHITECTURE 20), derived rather than wrapped so that a deque is the
// buffer itself. A push always succeeds, so PushFront and PushBack return nothing, and
// UnboundedCapacity's constant answer folds the buffer's capacity check away. The inherited
// TryPushFront/TryPushBack never refuse and IsFull never answers true: that is what an unbounded
// capacity means, not a second way to push.
/// <summary>A double-ended queue that adds, reads and removes elements at either end.</summary>
public sealed class Deque<Element>() : CircularBuffer<Element, UnboundedCapacity>(default)
{
    /// <summary>Adds an element at the front.</summary>
    /// <param name="item">The element to add.</param>
    public void PushFront(Element item) => _ = TryPushFront(item);

    /// <summary>Adds an element at the back.</summary>
    /// <param name="item">The element to add.</param>
    public void PushBack(Element item) => _ = TryPushBack(item);
}
