using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CriticalPointsWorkloads (ARCHITECTURE 17.7). The reading depends on a
// strictly sign-alternating chain, which forces almost every interior node to be a genuine local
// maximum or minimum - so both strategies do comparable real work instead of the list happening to
// be monotonic in places, which would let the baseline's List<int> stay nearly empty and hide the
// allocation the comparison is about.
public sealed partial class CriticalPointsWorkloadsTests
{
    private const int Length = 32;
    private const int Seed = 2058; // LC problem number
    private const int MinMagnitude = 1;
    private const int MaxMagnitude = 999;

    [Fact]
    public void BuildZigzagList_Length_ReturnsAChainOfThatManyNodes() =>
        Assert.Equal(Length, Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed)).Count);

    [Fact]
    public void BuildZigzagList_EveryPosition_CarriesTheAlternatingSign()
    {
        var values = Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed));

        for (var index = 0; index < Length; index++)
        {
            Assert.Equal(index % 2 == 0 ? 1 : -1, Math.Sign(values[index]));
        }
    }

    [Fact]
    public void BuildZigzagList_EveryValue_StaysInsideTheMagnitudeBand()
    {
        var values = Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed));

        Assert.All(values, value => Assert.InRange(Math.Abs(value), MinMagnitude, MaxMagnitude));
    }

    [Fact]
    public void BuildZigzagList_EveryInteriorNode_IsALocalMaximumOrMinimum()
    {
        var values = Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed));

        for (var index = 1; index < Length - 1; index++)
        {
            var isLocalMaximum = values[index] > values[index - 1] && values[index] > values[index + 1];
            var isLocalMinimum = values[index] < values[index - 1] && values[index] < values[index + 1];

            Assert.True(isLocalMaximum || isLocalMinimum);
        }
    }

    [Fact]
    public void BuildZigzagList_SameSeed_ReturnsTheSameChain() =>
        Assert.Equal(
            Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed)),
            Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed)));

    private static List<int> Values(SinglyLinkedListNode<int> head)
    {
        var values = new List<int>();

        for (SinglyLinkedListNode<int>? node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }
}
