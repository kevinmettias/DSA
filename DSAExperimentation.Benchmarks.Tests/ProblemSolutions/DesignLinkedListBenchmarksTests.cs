using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignLinkedListBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a List<int> inserting at the front, shifting every existing
// element, against this repo's own singly linked node chain's pointer reassignment - so a harness
// whose arms disagree is timing two different problems. There is no [GlobalSetup]: each arm
// constructs its own list and runs the same Calls-length sequence of head insertions.
public sealed partial class DesignLinkedListBenchmarksTests
{
    private const int SmallestCalls = 5_000;

    // Both arms return the call count itself rather than anything read back out of the list, so
    // agreement pins that both were driven over the same Calls-sized insertion sequence and no
    // more - the benchmark's return carries no per-insertion detail for a stronger claim to rest
    // on, which is why Calls starts in the thousands rather than at a size the list's own
    // contents would be cheap to check.
    [Fact]
    public void ArrayListAddAtHead_ThousandCallHeadInsertionRun_AgreesWithSinglyLinkedListChainAddAtHead()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SinglyLinkedListChainAddAtHead(), harness.ArrayListAddAtHead());
    }

    [Fact]
    public void SinglyLinkedListChainAddAtHead_ThousandCallHeadInsertionRun_AgreesWithArrayListAddAtHead()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayListAddAtHead(), harness.SinglyLinkedListChainAddAtHead());
    }

    private static DesignLinkedListBenchmarks BuildHarness() => new() { Calls = SmallestCalls };
}
