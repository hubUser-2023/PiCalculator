[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTest")]

namespace PiComputation;

internal static class PiCalculator
{
    private const long NUM_STEPS = 200000000;

    internal static double ComputePiSequential()
    {
        double sum = 0.0;
        for (long i = 0; i < NUM_STEPS; i++)
        {
            sum += 1.0 / (4 * i + 1) - 1.0 / (4 * i + 3);
        }
        return sum * 4.0;
    }

    internal static double ComputePiParallel(int numThreads)
    {
        double[] partialSums = new double[numThreads];
        Thread[] threads = new Thread[numThreads];

        for (int tid = 0; tid < numThreads; tid++)
        {
            int threadId = tid; 
            threads[tid] = new Thread(() =>
            {
                double sum = 0.0;
                for (long i = threadId; i < NUM_STEPS; i += numThreads)
                {
                    sum += 1.0 / (4 * i + 1) - 1.0 / (4 * i + 3);
                }
                partialSums[threadId] = sum;
            });
            threads[tid].Start();
        }

        for (int tid = 0; tid < numThreads; tid++)
        {
            threads[tid].Join();
        }

        double totalSum = 0.0;
        for (int i = 0; i < numThreads; i++)
        {
            totalSum += partialSums[i];
        }
        return totalSum * 4.0;
    }
}

internal static class Program
{
    internal static void Main(string[] args)
    {
        Console.WriteLine("Введите число:");
        string input = Console.ReadLine();

        bool isParsed = int.TryParse(input, out int numThreads);
        if (!isParsed || numThreads <= 0)
        {
            Console.WriteLine("Должно быть большо 0");
            Console.ReadLine();
            return;
        }

        double pi = PiCalculator.ComputePiParallel(numThreads);
        Console.WriteLine("pi = {0:F15}", pi);
        Console.ReadLine();
    }
}
