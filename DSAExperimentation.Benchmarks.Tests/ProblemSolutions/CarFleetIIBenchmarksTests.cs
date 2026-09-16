using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CarFleetIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a per-car forward scan that re-examines every car ahead
// against one monotonic stack sweep - so a harness whose arms disagree is timing two different
// problems. Answers come back one per car in car order, which is part of this answer: collision
// time i belongs to car i, so AnswerText.Of and not OfUnorderedSet is the rendering that keeps
// each time scored against its own car.
public sealed partial class CarFleetIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The sentinel both strategies report for a car that never collides - and the value every car
    // in this fleet gets. Setup builds cars[i] with position i * 10 and speed i + 1, so the car at
    // the back of each pair is always the slower of the two and nothing ever catches anything: a
    // rebuilt fleet must answer with this sentinel once per car. The fleet is a closed form over
    // Length with no draw from any stream, so the same Length is the whole of what pins it.
    private const double NoCollisionTime = -1;

    // Both arms run the identical catch-up division over the identical pair of cars, so they agree
    // far inside this; the constant is here so the two doubles are compared as measurements rather
    // than for exact equality.
    private const double CollisionTimeTolerance = 1e-9;

    [Fact]
    public void Setup_SameLength_RebuildsTheAllNoCollisionFleet()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Enumerable.Repeat(NoCollisionTime, SmallestLength)),
            AnswerText.Of(first.BruteForcePerCar()));
        Assert.Equal(
            AnswerText.Of(first.MonotonicStackSweep()),
            AnswerText.Of(second.MonotonicStackSweep()));
    }

    [Fact]
    public void BruteForcePerCar_CarsSlowerThanTheOnesAhead_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();
        var expected = harness.MonotonicStackSweep();
        var actual = harness.BruteForcePerCar();

        Assert.Equal(expected.Length, actual.Length);

        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index], actual[index], CollisionTimeTolerance);
        }
    }

    [Fact]
    public void MonotonicStackSweep_CarsSlowerThanTheOnesAhead_AgreesWithBruteForcePerCar()
    {
        var harness = BuildHarness();
        var expected = harness.BruteForcePerCar();
        var actual = harness.MonotonicStackSweep();

        Assert.Equal(expected.Length, actual.Length);

        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index], actual[index], CollisionTimeTolerance);
        }
    }

    private static CarFleetIIBenchmarks BuildHarness()
    {
        var harness = new CarFleetIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
