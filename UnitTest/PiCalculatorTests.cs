using PiComputation;

namespace UnitTest;
public class PiCalculatorTests
{
    private const double Tolerance = 1e-6;

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void ComputePiParallel_ShouldBeCloseToSequential(int numThreads)
    {
        double serialPi = PiCalculator.ComputePiSequential();
        double parallelPi = PiCalculator.ComputePiParallel(numThreads);

        Assert.True(Math.Abs(serialPi - parallelPi) <= Tolerance,
            $"For {numThreads} threads: serial pi = {serialPi:F15}, parallel pi = {parallelPi:F15}");
    }
}