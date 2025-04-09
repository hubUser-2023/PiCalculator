using System.Diagnostics;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTest")]

namespace PiComputation;

internal static class PiCalculator
{
    private const long NUM_STEPS = 200000000;

    internal static double ComputePiParallel(int numThreads)
    {
        double[] partialSums = new double[numThreads];
        Thread[] threads = new Thread[numThreads];

        long chunk = NUM_STEPS / numThreads;

        for (int tid = 0; tid < numThreads; tid++)
        {
            long start = tid * chunk;
            long end = (tid == numThreads - 1) ? NUM_STEPS : start + chunk;
            int threadIndex = tid;
            threads[tid] = new Thread(() =>
            {
                double sum = 0.0;
                for (long i = start; i < end; i ++)
                {
                    sum += 1.0 / (4 * i + 1) - 1.0 / (4 * i + 3);
                }
                partialSums[threadIndex] = sum;
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
        Console.WriteLine("Введите число потоков или слово test для диагностики");
        string? input = Console.ReadLine().Trim();
        
        if (input.ToLower() == "test")
        {
            RunAutomaticMeasurements();
            return;
        }

        bool isParsed = int.TryParse(input, out int numThreads);
        if (!isParsed || numThreads <= 0)
        {
            Console.WriteLine("Должно быть большо 0");
            Console.ReadLine();
            return;
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        double pi = PiCalculator.ComputePiParallel(numThreads);
        stopwatch.Stop();

        Console.WriteLine("pi = {0:F15}", pi);
        Console.ReadLine();
    }

    private static void RunAutomaticMeasurements()
    {
        int maxThreads = 100;
        string fileName = "DiagnosticInfo.csv";

        using (StreamWriter swFile = new StreamWriter(fileName))
        {
            swFile.WriteLine("Threads, ElapsedMilliseconds, PiValue");

            for (int threads = 1; threads <= maxThreads; threads++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                double pi = PiCalculator.ComputePiParallel(threads);
                stopwatch.Stop();

                swFile.WriteLine($"{threads},{stopwatch.ElapsedMilliseconds},{pi:F15}");
                Console.WriteLine($"Потоков: {threads}, Время: {stopwatch.ElapsedMilliseconds} ms, pi = {pi: F15}"); 
            }
        }
        
    Console.WriteLine($"Готово{fileName}");
    Console.ReadLine();
    }
}
