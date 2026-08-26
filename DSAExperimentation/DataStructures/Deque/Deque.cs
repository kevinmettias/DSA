
namespace DSAExperimentation.DataStructures.Deque;

internal sealed class Deque<Element>
{
    private const string EmptyDequeMessage = "The deque contains no elements.";

    private readonly CircularBuffer<Element> _items = new();

    public int Count => _items.Count;

    public void PushFront(Element item) => _items.AddFront(item);

    public void PushBack(Element item) => _items.AddBack(item);

    public Element PeekFront()
        => Count == 0
            ? ThrowEmptyDeque()
            : _items.GetFront();

    public Element PeekBack()
        => Count == 0
            ? ThrowEmptyDeque()
            : _items.GetBack();

    public bool TryPeekFront(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetFront();
        return true;
    }

    public bool TryPeekBack(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetBack();
        return true;
    }

    public Element PopFront()
    {
        var item = PeekFront();
        _items.RemoveFront();
        return item;
    }

    public Element PopBack()
    {
        var item = PeekBack();
        _items.RemoveBack();
        return item;
    }

    public bool TryPopFront(out Element item)
    {
        if (!TryPeekFront(out item))
        {
            return false;
        }

        _items.RemoveFront();
        return true;
    }

    public bool TryPopBack(out Element item)
    {
        if (!TryPeekBack(out item))
        {
            return false;
        }

        _items.RemoveBack();
        return true;
    }

    private static Element ThrowEmptyDeque() => throw new InvalidOperationException(EmptyDequeMessage);
}
