using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ReconstructItinerary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReconstructItinerarySolution's, the same methods
// ReconstructItineraryTests proves correct, run against a synthetic ticket graph large
// enough to separate LinearScanSelection's O(k) scan-and-remove from HeapSelection's
// O(log k) repo Heap push/pop.
[MemoryDiagnoser]
public class ReconstructItineraryBenchmarks
{
    // LC problem number, reused as the deterministic ticket seed.
    private const int TicketSeed = 332;

    [Params(200, 2_000)]
    public int TicketCount;

    private string[][] _tickets = null!;

    [GlobalSetup]
    public void Setup() => _tickets = ItineraryWorkloads.BuildTickets(TicketCount, seed: TicketSeed);

    [Benchmark(Baseline = true)]
    public List<string> LinearScanSelection() => ReconstructItinerarySolution.FindItineraryByLinearScan(_tickets);

    [Benchmark]
    public List<string> HeapSelection() => ReconstructItinerarySolution.FindItineraryByHeapSelection(_tickets);
}
