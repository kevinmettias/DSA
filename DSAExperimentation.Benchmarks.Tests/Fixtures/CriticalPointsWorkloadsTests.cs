using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CriticalPointsWorkloads (ARCHITECTURE 17.7). The reading depends on a chain
// that strictly alternates between a high band and a low one, which forces almost every interior node
// to be a genuine local maximum or minimum - so both strategies do comparable real work instead of
// the list happening to be monotonic in places, which would let the baseline's List<int> stay nearly
// empty and hide the allocation the comparison is about - and on every value staying inside LC 2058's
// 1..10^5.
public sealed partial class CriticalPointsWorkloadsTests
{
    private const int Length = 32;
    private const int Seed = 2058; // LC problem number
    private const int PeakPositionModulus = 2; // mirrors CriticalPointsWorkloads.PeakPositionModulus

    // The two bands the chain alternates between: peaks at even positions, valleys at odd ones.
    private const int LowestValley = 1;
    private const int HighestValley = 999;
    private const int LowestPeak = 1_001;
    private const int HighestPeak = 1_999;

    // LC 2058's node values.
    private const int LowestNodeValue = 1;
    private const int HighestNodeValue = 100_000;

    [Fact]
    public void BuildZigzagList_Length_ReturnsAChainOfThatManyNodes() =>
        Assert.Equal(Length, Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed)).Count);

    [Fact]
    public void BuildZigzagList_EveryPosition_CarriesItsAlternatingBand()
    {
        var values = Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed));

        for (var index = 0; index < Length; index++)
        {
            var isPeakPosition = index % PeakPositionModulus == 0;

            if (isPeakPosition)
            {
                Assert.InRange(values[index], LowestPeak, HighestPeak);
            }
            else
            {
                Assert.InRange(values[index], LowestValley, HighestValley);
            }
        }
    }

    [Fact]
    public void BuildZigzagList_EveryValue_StaysInsideLeetCodesNodeRange()
    {
        var values = Values(CriticalPointsWorkloads.BuildZigzagList(Length, Seed));

        Assert.All(values, value => Assert.InRange(value, LowestNodeValue, HighestNodeValue));
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
