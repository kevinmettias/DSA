using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertGreatestCommonDivisorsInLinkedListBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on the smallest workload, so this asserts
// only the sequence derived from the fixture rather than read back out of an arm. The baseline
// returns the resulting list's head; the expected answer is stated as plain values and a linked
// node is not an enumerable sequence, so the result is walked into its values first.
// [GlobalSetup] builds the values 1..Length in order (wrapping only past LC 2807's 1,000 value
// cap, beyond the smallest length), and two consecutive integers always have a greatest common
// divisor of one, so every inserted node holds one and the answer is fixed.
public sealed partial class InsertGreatestCommonDivisorsInLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // gcd(n, n + 1) is one for every n, so the fixture's consecutive values fix every inserted
    // gcd without any Euclidean step.
    private const int ConsecutiveGcd = 1;

    // [GlobalSetup] builds the values 1..Length, and the run's first is seeded directly, so the loop
    // over the rest starts at its second.
    private const int SecondWorkloadValue = 2;

    [Fact]
    public void ValueRebuild_ConsecutiveValues_InterleavesAGcdOfOneBetweenEveryPair() =>
        Assert.Equal(ExpectedGcdInterleaved(SmallestLength), ValuesOf(BuildHarness().ValueRebuild()));

    private static InsertGreatestCommonDivisorsInLinkedListBenchmarks BuildHarness()
    {
        var harness = new InsertGreatestCommonDivisorsInLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static int[] ValuesOf(SinglyLinkedListNode<int>? answer)
    {
        var values = new List<int>();

        for (var node = answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // nums[0], gcd(nums[0], nums[1]), nums[1], ... - every value except the first is preceded by
    // the gcd of its pair, which the fixture's consecutive values make a constant one.
    private static int[] ExpectedGcdInterleaved(int length)
    {
        var values = new List<int> { 1 };

        for (var value = SecondWorkloadValue; value <= length; value++)
        {
            values.Add(ConsecutiveGcd);
            values.Add(value);
        }

        return [.. values];
    }
}
