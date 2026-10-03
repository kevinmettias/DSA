using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.TimeNeededToBuyTickets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TimeNeededToBuyTicketsSolution's, the same methods
// TimeNeededToBuyTicketsSolutionTests proves correct - this repo's Queue<(int, int)>
// replaying the line, O(total tickets sold before targetPerson finishes), against
// the O(n) closed-form sum. _tickets draws each person's count from the top half of
// LC 2073's [1, 100], so the simulation is forced through its full
// O(n * ticketsPerPerson) worst case instead of finishing after a handful of turns,
// and Length stops at its 100 people.
public class TimeNeededToBuyTicketsBenchmarks
{
    private const int RandomSeed = 2073; // LC problem number
    private const int MinTickets = 50;
    private const int MaxTickets = 100;

    private int[] _tickets = [];

    private int _targetPerson;
    [Params(50, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tickets = SeededDraws.Values(Length, MinTickets, MaxTickets + 1, random);
        _targetPerson = Length / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public int QueueSimulation() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByQueueSimulation(_tickets, _targetPerson);

    [Benchmark]
    public int ClosedFormSum() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByClosedFormSum(_tickets, _targetPerson);
}
